using InsureFlow.Application.Abstractions;
using InsureFlow.Application.Persistence;
using InsureFlow.Application.Tenants;
using InsureFlow.Contracts.Authorizations;
using InsureFlow.Domain.Entities;

namespace InsureFlow.Application.Authorizations;

public sealed class AuthorizationService(
    IAuthorizationRepository repo,
    IPatientRepository patients,
    IPatientInsuranceRepository insurances,
    IPayerLookupRepository payers,
    IProviderRepository providers,
    ITenantContext tenant,
    IAuditService audit) : IAuthorizationService
{
    public async Task<IReadOnlyList<AuthorizationDto>> GetPagedAsync(
        AuthorizationListRequest r,
        CancellationToken ct = default)
    {
        var p = r.Page < 1 ? 1 : r.Page;

        var s = r.PageSize switch
        {
            < 1 => 25,
            > 100 => 100,
            _ => r.PageSize
        };

        var rows = await repo.GetPagedAsync(
            r.Search,
            r.Status,
            r.Priority,
            p,
            s,
            ct);

        var result = new List<AuthorizationDto>();

        foreach (var row in rows)
            result.Add(await MapAsync(row, ct));

        return result;
    }

    public async Task<AuthorizationDto?> GetByIdAsync(
        Guid id,
        CancellationToken ct = default)
    {
        var x = await repo.GetByExternalIdAsync(id, ct);

        return x is null
            ? null
            : await MapAsync(x, ct);
    }

    public async Task<AuthorizationDto> CreateAsync(
        CreateAuthorizationRequest r,
        CancellationToken ct = default)
    {
        var tid = RequireTenant();

        if (string.IsNullOrWhiteSpace(r.AuthorizationNumber))
            throw new ArgumentException(
                "Authorization number is required.");

        var patient =
            await patients.GetByExternalIdAsync(r.PatientId, ct)
            ?? throw new InvalidOperationException(
                "Patient was not found for the current tenant.");

        var pi =
            await insurances.GetByExternalIdAsync(
                r.PatientInsuranceId,
                ct)
            ?? throw new InvalidOperationException(
                "Patient insurance was not found for the current tenant.");

        if (pi.PatientId != patient.Id)
            throw new InvalidOperationException(
                "Patient insurance does not belong to the specified patient.");

        var payer =
            await payers.GetByExternalIdAsync(r.PayerId, ct)
            ?? throw new InvalidOperationException(
                "Payer was not found for the current tenant.");

        var provider =
            await providers.GetByExternalIdAsync(r.ProviderId, ct)
            ?? throw new InvalidOperationException(
                "Provider was not found for the current tenant.");

        if (pi.PayerId != payer.Id)
            throw new InvalidOperationException(
                "Patient insurance payer does not match authorization payer.");

        if (await repo.ExistsByNumberAsync(
                r.AuthorizationNumber.Trim(),
                null,
                ct))
        {
            throw new InvalidOperationException(
                "Authorization number already exists.");
        }

        var x = new Authorization(
            tid,
            patient.Id,
            pi.Id,
            payer.Id,
            provider.Id,
            r.AuthorizationNumber,
            r.RequestedFrom,
            r.RequestedTo);

        x.UpdateDetails(
            r.RequestType,
            r.Priority,
            r.PlaceOfService,
            r.RequestedFrom,
            r.RequestedTo,
            r.ClinicalSummary);

        await repo.AddAsync(x, ct);
        await repo.SaveChangesAsync(ct);

        await AddHistoryAsync(
            x,
            "",
            "DRAFT",
            "Authorization created",
            ct);

        await repo.SaveChangesAsync(ct);

        await AuditAsync(
            tid,
            "AuthorizationCreated",
            x,
            ct);

        return await MapAsync(x, ct);
    }

    public async Task<bool> UpdateAsync(
        Guid id,
        UpdateAuthorizationRequest r,
        CancellationToken ct = default)
    {
        var tid = RequireTenant();

        var x = await repo.GetByExternalIdAsync(id, ct);

        if (x is null)
            return false;

        if (x.Status != "DRAFT")
            throw new InvalidOperationException(
                "Only draft authorizations can be edited.");

        x.UpdateDetails(
            r.RequestType,
            r.Priority,
            r.PlaceOfService,
            r.RequestedFrom,
            r.RequestedTo,
            r.ClinicalSummary);

        await repo.UpdateAsync(x, ct);
        await repo.SaveChangesAsync(ct);

        await AuditAsync(
            tid,
            "AuthorizationUpdated",
            x,
            ct);

        return true;
    }

    public Task<bool> SubmitAsync(
        Guid id,
        string? note,
        CancellationToken ct = default)
        => TransitionAsync(
            id,
            "PENDING",
            note,
            new[] { "DRAFT" },
            ct);

    public Task<bool> ApproveAsync(
        Guid id,
        string? note,
        CancellationToken ct = default)
        => TransitionAsync(
            id,
            "APPROVED",
            note,
            new[] { "PENDING" },
            ct);

    public Task<bool> DenyAsync(
        Guid id,
        string reason,
        CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(reason))
            throw new ArgumentException(
                "A denial reason is required.",
                nameof(reason));

        return TransitionAsync(
            id,
            "DENIED",
            reason,
            new[] { "PENDING" },
            ct);
    }

    public Task<bool> CancelAsync(
        Guid id,
        string? reason,
        CancellationToken ct = default)
        => TransitionAsync(
            id,
            "CANCELLED",
            reason,
            new[] { "DRAFT", "PENDING", "APPROVED" },
            ct);

    private async Task<bool> TransitionAsync(
        Guid id,
        string target,
        string? note,
        string[] allowed,
        CancellationToken ct)
    {
        var tid = RequireTenant();

        var x = await repo.GetByExternalIdAsync(id, ct);

        if (x is null)
            return false;

        if (!allowed.Contains(
                x.Status,
                StringComparer.Ordinal))
        {
            throw new InvalidOperationException(
                $"Cannot change authorization from {x.Status} to {target}.");
        }

        if (target == "PENDING" &&
            (await repo.GetServicesAsync(x.Id, ct)).Count == 0)
        {
            throw new InvalidOperationException(
                "Add at least one requested service before submitting an authorization.");
        }

        var from = x.Status;

        x.ChangeStatus(
            target,
            note);

        await repo.UpdateAsync(x, ct);

        await AddHistoryAsync(
            x,
            from,
            target,
            note,
            ct);

        await repo.SaveChangesAsync(ct);

        await AuditAsync(
            tid,
            $"Authorization{target}",
            x,
            ct,
            note);

        return true;
    }

    public async Task<IReadOnlyList<AuthorizationServiceLineDto>>
        GetServicesAsync(
            Guid id,
            CancellationToken ct = default)
    {
        var x = await RequiredAsync(id, ct);

        return (await repo.GetServicesAsync(x.Id, ct))
            .Select(Map)
            .ToList();
    }

    public async Task<AuthorizationServiceLineDto> AddServiceAsync(
        Guid id,
        AddAuthorizationServiceLineRequest r,
        CancellationToken ct = default)
    {
        var tid = RequireTenant();

        var x = await RequiredAsync(id, ct);

        if (x.Status != "DRAFT")
            throw new InvalidOperationException(
                "Services can only be added while the authorization is in DRAFT status.");

        if (string.IsNullOrWhiteSpace(r.CptHcpcsCode))
            throw new ArgumentException(
                "CPT/HCPCS code is required.");

        if (r.UnitsRequested < 1)
            throw new ArgumentException(
                "Units requested must be at least one.");

        if (r.RequestedAmount < 0)
            throw new ArgumentException(
                "Requested amount cannot be negative.");

        var line = new AuthorizationServiceLine(
            tid,
            x.Id,
            r.CptHcpcsCode,
            r.UnitsRequested);

        line.Update(
            r.UnitsRequested,
            null,
            r.RequestedAmount,
            null,
            "REQUESTED");

        await repo.AddServiceAsync(
            line,
            ct);

        await repo.SaveChangesAsync(ct);

        return Map(line);
    }

    public async Task<bool> UpdateServiceAsync(
        Guid id,
        long serviceId,
        UpdateAuthorizationServiceLineRequest r,
        CancellationToken ct = default)
    {
        var x = await repo.GetByExternalIdAsync(id, ct);

        if (x is null)
            return false;

        if (x.Status != "DRAFT")
            throw new InvalidOperationException(
                "Services can only be changed while the authorization is in DRAFT status.");

        var line = await repo.GetServiceAsync(
            serviceId,
            ct);

        if (line is null ||
            line.AuthorizationId != x.Id)
        {
            return false;
        }

        if (r.UnitsApproved is < 0 ||
            r.RequestedAmount is < 0 ||
            r.ApprovedAmount is < 0)
        {
            throw new ArgumentException(
                "Units and amounts cannot be negative.");
        }

        line.Update(
            r.UnitsRequested,
            r.UnitsApproved,
            r.RequestedAmount,
            r.ApprovedAmount,
            r.Status);

        await repo.SaveChangesAsync(ct);

        return true;
    }

    public async Task<bool> DeleteServiceAsync(
        Guid id,
        long serviceId,
        CancellationToken ct = default)
    {
        var x = await repo.GetByExternalIdAsync(id, ct);

        if (x is null)
            return false;

        if (x.Status != "DRAFT")
            throw new InvalidOperationException(
                "Services can only be deleted while the authorization is in DRAFT status.");

        var ok = await repo.DeleteServiceAsync(
            x.Id,
            serviceId,
            ct);

        if (ok)
            await repo.SaveChangesAsync(ct);

        return ok;
    }

    public async Task<IReadOnlyList<AuthorizationDiagnosisDto>>
        GetDiagnosesAsync(
            Guid id,
            CancellationToken ct = default)
    {
        var x = await RequiredAsync(id, ct);

        return (await repo.GetDiagnosesAsync(x.Id, ct))
            .Select(Map)
            .ToList();
    }

    public async Task<AuthorizationDiagnosisDto> AddDiagnosisAsync(
        Guid id,
        AddAuthorizationDiagnosisRequest r,
        CancellationToken ct = default)
    {
        var tid = RequireTenant();

        var x = await RequiredAsync(id, ct);

        if (x.Status != "DRAFT")
            throw new InvalidOperationException(
                "Diagnoses can only be added while the authorization is in DRAFT status.");

        if (string.IsNullOrWhiteSpace(r.Icd10Code) ||
            r.Sequence < 1)
        {
            throw new ArgumentException(
                "A diagnosis code and positive sequence are required.");
        }

        var d = new AuthorizationDiagnosis(
            tid,
            x.Id,
            r.Icd10Code,
            r.Description,
            r.Sequence);

        await repo.AddDiagnosisAsync(
            d,
            ct);

        await repo.SaveChangesAsync(ct);

        return Map(d);
    }

    public async Task<bool> DeleteDiagnosisAsync(
        Guid id,
        long diagnosisId,
        CancellationToken ct = default)
    {
        var x = await repo.GetByExternalIdAsync(id, ct);

        if (x is null)
            return false;

        if (x.Status != "DRAFT")
            throw new InvalidOperationException(
                "Diagnoses can only be deleted while the authorization is in DRAFT status.");

        var ok = await repo.DeleteDiagnosisAsync(
            x.Id,
            diagnosisId,
            ct);

        if (ok)
            await repo.SaveChangesAsync(ct);

        return ok;
    }

    public async Task<IReadOnlyList<AuthorizationDocumentDto>>
        GetDocumentsAsync(
            Guid id,
            CancellationToken ct = default)
    {
        var x = await RequiredAsync(id, ct);

        return (await repo.GetDocumentsAsync(x.Id, ct))
            .Select(Map)
            .ToList();
    }

    public async Task<AuthorizationDocumentDto> AddDocumentAsync(
        Guid id,
        AddAuthorizationDocumentRequest r,
        CancellationToken ct = default)
    {
        var tid = RequireTenant();

        var x = await RequiredAsync(id, ct);

        if (string.IsNullOrWhiteSpace(r.FileName) ||
            string.IsNullOrWhiteSpace(r.StorageKey))
        {
            throw new ArgumentException(
                "FileName and StorageKey are required. Upload the file to approved secure storage before registering its metadata.");
        }

        if (r.FileSize < 0)
            throw new ArgumentException(
                "File size cannot be negative.");

        var d = new AuthorizationDocument(
            tid,
            x.Id,
            r.FileName,
            r.StorageKey,
            r.ContentType,
            r.FileSize,
            r.DocumentType);

        await repo.AddDocumentAsync(
            d,
            ct);

        await repo.SaveChangesAsync(ct);

        return Map(d);
    }

    public async Task<bool> DeleteDocumentAsync(
        Guid id,
        long documentId,
        CancellationToken ct = default)
    {
        var x = await repo.GetByExternalIdAsync(id, ct);

        if (x is null)
            return false;

        var ok = await repo.DeleteDocumentAsync(
            x.Id,
            documentId,
            ct);

        if (ok)
            await repo.SaveChangesAsync(ct);

        return ok;
    }

    public async Task<IReadOnlyList<AuthorizationNoteDto>>
        GetNotesAsync(
            Guid id,
            CancellationToken ct = default)
    {
        var x = await RequiredAsync(id, ct);

        return (await repo.GetNotesAsync(x.Id, ct))
            .Select(n => new AuthorizationNoteDto(
                n.Id,
                n.NoteType,
                n.NoteText,
                n.CreatedAt))
            .ToList();
    }

    public async Task<AuthorizationNoteDto> AddNoteAsync(
        Guid id,
        AddAuthorizationNoteRequest r,
        CancellationToken ct = default)
    {
        var tid = RequireTenant();

        var x = await RequiredAsync(id, ct);

        if (string.IsNullOrWhiteSpace(r.NoteText))
            throw new ArgumentException(
                "Note text is required.");

        var n = new AuthorizationNote(
            tid,
            x.Id,
            r.NoteType,
            r.NoteText);

        await repo.AddNoteAsync(
            n,
            ct);

        await repo.SaveChangesAsync(ct);

        await AuditAsync(
            tid,
            "AuthorizationNoteAdded",
            x,
            ct);

        return new AuthorizationNoteDto(
            n.Id,
            n.NoteType,
            n.NoteText,
            n.CreatedAt);
    }

    public async Task<IReadOnlyList<AuthorizationHistoryDto>>
        GetHistoryAsync(
            Guid id,
            CancellationToken ct = default)
    {
        var x = await RequiredAsync(id, ct);

        return (await repo.GetHistoryAsync(x.Id, ct))
            .Select(h => new AuthorizationHistoryDto(
                h.Id,
                h.FromStatus,
                h.ToStatus,
                h.Note,
                h.ChangedAt,
                h.ChangedByUserId))
            .ToList();
    }

    public async Task<AuthorizationAppealDto> CreateAppealAsync(
        Guid id,
        CreateAuthorizationAppealRequest r,
        CancellationToken ct = default)
    {
        var tid = RequireTenant();

        var x = await RequiredAsync(id, ct);

        if (x.Status != "DENIED")
            throw new InvalidOperationException(
                "Only denied authorizations can be appealed.");

        if (string.IsNullOrWhiteSpace(r.Reason))
            throw new ArgumentException(
                "Appeal reason is required.");

        var appeal = new AuthorizationAppeal(
            tid,
            x.Id,
            r.Reason,
            r.Notes,
            DateTime.UtcNow);

        await repo.AddAppealAsync(
            appeal,
            ct);

        x.ChangeStatus(
            "APPEALED",
            r.Reason);

        await repo.UpdateAsync(
            x,
            ct);

        await AddHistoryAsync(
            x,
            "DENIED",
            "APPEALED",
            r.Reason,
            ct);

        await repo.SaveChangesAsync(ct);

        await AuditAsync(
            tid,
            "AuthorizationAppealed",
            x,
            ct,
            r.Reason);

        return new(
            appeal.Id,
            appeal.Reason,
            appeal.Notes,
            appeal.Status,
            appeal.SubmittedAt);
    }

    private async Task<Authorization> RequiredAsync(
        Guid id,
        CancellationToken ct)
        => await repo.GetByExternalIdAsync(id, ct)
           ?? throw new InvalidOperationException(
               "Authorization was not found for the current tenant.");

    private async Task AddHistoryAsync(
        Authorization x,
        string from,
        string to,
        string? note,
        CancellationToken ct)
        => await repo.AddHistoryAsync(
            new AuthorizationHistory(
                x.TenantId,
                x.Id,
                from,
                to,
                note,
                DateTime.UtcNow),
            ct);

    private async Task AuditAsync(
        long tid,
        string action,
        Authorization x,
        CancellationToken ct,
        string? reason = null)
        => await audit.WriteAsync(
            new AuditEntry(
                tid,
                null,
                action,
                nameof(Authorization),
                x.ExternalId.ToString(),
                Reason: reason),
            ct);

    private long RequireTenant()
        => tenant.TenantId
           ?? throw new InvalidOperationException(
               "A tenant context is required for this operation.");

    private async Task<AuthorizationDto> MapAsync(
        Authorization x,
        CancellationToken ct)
    {
        var pi = await insurances.GetByIdAsync(
            x.PatientInsuranceId,
            ct);

        var provider = await providers.GetByIdAsync(
            x.ProviderId,
            ct);

        return new(
            x.ExternalId,
            x.TenantId,
            pi?.Patient.ExternalId ?? Guid.Empty,
            pi?.ExternalId ?? Guid.Empty,
            pi?.Payer.ExternalId ?? Guid.Empty,
            provider?.ExternalId ?? Guid.Empty,
            x.AuthorizationNumber,
            x.RequestType,
            x.Status,
            x.Priority,
            x.PlaceOfService,
            x.RequestedFrom,
            x.RequestedTo,
            x.DecisionDate,
            x.PayerReference,
            x.ClinicalSummary,
            x.DecisionReason,
            x.CreatedAtUtc,
            x.UpdatedAtUtc);
    }

    private static AuthorizationServiceLineDto Map(
        AuthorizationServiceLine x)
        => new(
            x.Id,
            x.CptHcpcsCode,
            x.Modifier,
            x.DiagnosisCode,
            x.UnitsRequested,
            x.UnitsApproved,
            x.RequestedAmount,
            x.ApprovedAmount,
            x.Status);

    private static AuthorizationDiagnosisDto Map(
        AuthorizationDiagnosis x)
        => new(
            x.Id,
            x.Icd10Code,
            x.Description,
            x.Sequence);

    private static AuthorizationDocumentDto Map(
        AuthorizationDocument x)
        => new(
            x.Id,
            x.FileName,
            x.StorageKey,
            x.ContentType,
            x.FileSize,
            x.DocumentType,
            x.UploadedAt);
}