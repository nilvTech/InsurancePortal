
using InsureFlow.Application.Abstractions;
using InsureFlow.Application.Persistence;
using InsureFlow.Application.Tenants;
using InsureFlow.Contracts.Claims;
using InsureFlow.Domain.Entities;

namespace InsureFlow.Application.Claims;

public sealed class ClaimService : IClaimService
{
    private readonly IClaimRepository _claimRepository;
    private readonly IPatientRepository _patientRepository;
    private readonly IPatientInsuranceRepository _patientInsuranceRepository;
    private readonly IPayerRepository _payerRepository;
    private readonly IProviderRepository _providerRepository;
    private readonly ITenantContext _tenantContext;
    private readonly IAuditService _auditService;

    public ClaimService(
        IClaimRepository claimRepository,
        IPatientRepository patientRepository,
        IPatientInsuranceRepository patientInsuranceRepository,
        IPayerRepository payerRepository,
        IProviderRepository providerRepository,
        ITenantContext tenantContext,
        IAuditService auditService)
    {
        _claimRepository = claimRepository;
        _patientRepository = patientRepository;
        _patientInsuranceRepository = patientInsuranceRepository;
        _payerRepository = payerRepository;
        _providerRepository = providerRepository;
        _tenantContext = tenantContext;
        _auditService = auditService;
    }

    public async Task<IReadOnlyList<ClaimDto>> GetPagedAsync(
        ClaimListRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        RequireTenant();

        var page = Math.Max(1, request.Page);
        var pageSize = Math.Clamp(request.PageSize, 1, 100);

        long? patientId = null;
        long? payerId = null;

        if (request.PatientId.HasValue)
        {
            var patient = await _patientRepository.GetByExternalIdAsync(
                request.PatientId.Value, cancellationToken);

            if (patient is null)
                return Array.Empty<ClaimDto>();

            patientId = patient.Id;
        }

        if (request.PayerId.HasValue)
        {
            var payer = await _payerRepository.GetByExternalIdAsync(
                request.PayerId.Value, cancellationToken);

            if (payer is null)
                return Array.Empty<ClaimDto>();

            payerId = payer.Id;
        }

        var claims = await _claimRepository.GetPagedAsync(
            request.Search,
            NormalizeOptional(request.Status)?.ToUpperInvariant(),
            NormalizeOptional(request.ClaimType)?.ToUpperInvariant(),
            patientId,
            payerId,
            request.ServiceFrom,
            request.ServiceTo,
            page,
            pageSize,
            cancellationToken);

        var result = new List<ClaimDto>(claims.Count);

        foreach (var claim in claims)
        {
            var lines = await _claimRepository.GetLinesByClaimIdAsync(
                claim.Id, cancellationToken);

            result.Add(await MapToDtoAsync(claim, lines, cancellationToken));
        }

        return result;
    }

    public async Task<ClaimDto?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        RequireTenant();

        var claim = await FindClaimAsync(id, cancellationToken);

        if (claim is null)
            return null;

        var lines = await _claimRepository.GetLinesByClaimIdAsync(
            claim.Id, cancellationToken);

        return await MapToDtoAsync(claim, lines, cancellationToken);
    }

    public async Task<ClaimDto> CreateAsync(
        CreateClaimRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var tenantId = RequireTenant();

        ValidateRequiredGuid(request.PatientId, nameof(request.PatientId));
        ValidateRequiredGuid(
            request.PatientInsuranceId, nameof(request.PatientInsuranceId));
        ValidateRequiredGuid(request.PayerId, nameof(request.PayerId));
        ValidateRequiredGuid(request.ProviderId, nameof(request.ProviderId));

        var claimNumber = NormalizeRequired(
            request.ClaimNumber, nameof(request.ClaimNumber));

        var claimType = NormalizeRequired(
            request.ClaimType, nameof(request.ClaimType)).ToUpperInvariant();

        ValidateServiceDates(request.ServiceFrom, request.ServiceTo);
        ValidateLines(request.Lines, request.ServiceFrom, request.ServiceTo);

        var patient = await _patientRepository.GetByExternalIdAsync(
            request.PatientId, cancellationToken);

        if (patient is null || patient.TenantId != tenantId)
            throw new ArgumentException("Patient was not found in the current tenant.");

        var insurance = await _patientInsuranceRepository.GetByExternalIdAsync(
            request.PatientInsuranceId, cancellationToken);

        if (insurance is null || insurance.TenantId != tenantId)
            throw new ArgumentException(
                "Patient insurance was not found in the current tenant.");

        if (insurance.PatientId != patient.Id)
            throw new ArgumentException(
                "The selected insurance does not belong to the selected patient.");

        var payer = await _payerRepository.GetByExternalIdAsync(
            request.PayerId, cancellationToken);

        if (payer is null || payer.TenantId != tenantId)
            throw new ArgumentException("Payer was not found in the current tenant.");

        if (insurance.PayerId != payer.Id)
            throw new ArgumentException(
                "The selected payer does not match the patient's insurance.");

        var provider = await _providerRepository.GetByExternalIdAsync(
            request.ProviderId, cancellationToken);

        if (provider is null || provider.TenantId != tenantId)
            throw new ArgumentException(
                "Provider was not found in the current tenant.");

        if (request.Lines is not null)
            ValidateLines(request.Lines, request.ServiceFrom, request.ServiceTo);

        var claim = new Claim(
            tenantId,
            patient.Id,
            insurance.Id,
            payer.Id,
            provider.Id,
            claimNumber,
            claimType,
            request.ServiceFrom,
            request.ServiceTo);

        // Persist the parent first so the database claim ID is available.
        await _claimRepository.AddAsync(claim, cancellationToken);
        await _claimRepository.SaveChangesAsync(cancellationToken);
        await _claimRepository.AddHistoryAsync(
            new ClaimHistory(
                tenantId,
                claim.Id,
                "NONE",
                claim.Status,
                "Claim created.",
                DateTime.UtcNow),
            cancellationToken);
        var lines = BuildLines(tenantId, claim.Id, request.Lines);
        claim.UpdateTotalChargeAmount(lines.Sum(x => x.ChargeAmount));

        if (lines.Count > 0)
            await _claimRepository.AddLinesAsync(lines, cancellationToken);

        await _claimRepository.UpdateAsync(claim, cancellationToken);
        await _claimRepository.SaveChangesAsync(cancellationToken);

        await AuditAsync(
            tenantId,
            "CLAIM_CREATED",
            claim.ExternalId,
            cancellationToken);

        return await MapToDtoAsync(claim, lines, cancellationToken);
    }

    public async Task<bool> UpdateAsync(
        Guid id,
        UpdateClaimRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var tenantId = RequireTenant();
        var claim = await FindClaimAsync(id, cancellationToken);

        if (claim is null)
            return false;

        if (claim.Status != "DRAFT")
            throw new InvalidOperationException(
                "Only draft claims can be updated.");

        ValidateRequiredGuid(request.PatientId, nameof(request.PatientId));
        ValidateRequiredGuid(
            request.PatientInsuranceId, nameof(request.PatientInsuranceId));
        ValidateRequiredGuid(request.PayerId, nameof(request.PayerId));
        ValidateRequiredGuid(request.ProviderId, nameof(request.ProviderId));

        var claimType = NormalizeRequired(
            request.ClaimType, nameof(request.ClaimType)).ToUpperInvariant();

        ValidateServiceDates(request.ServiceFrom, request.ServiceTo);
        ValidateLines(request.Lines, request.ServiceFrom, request.ServiceTo);

        var patient = await _patientRepository.GetByExternalIdAsync(
            request.PatientId, cancellationToken);

        if (patient is null || patient.TenantId != tenantId)
            throw new ArgumentException("Patient was not found in the current tenant.");

        var insurance = await _patientInsuranceRepository.GetByExternalIdAsync(
            request.PatientInsuranceId, cancellationToken);

        if (insurance is null || insurance.TenantId != tenantId)
            throw new ArgumentException(
                "Patient insurance was not found in the current tenant.");

        if (insurance.PatientId != patient.Id)
            throw new ArgumentException(
                "The selected insurance does not belong to the selected patient.");

        var payer = await _payerRepository.GetByExternalIdAsync(
            request.PayerId, cancellationToken);

        if (payer is null || payer.TenantId != tenantId)
            throw new ArgumentException("Payer was not found in the current tenant.");

        if (insurance.PayerId != payer.Id)
            throw new ArgumentException(
                "The selected payer does not match the patient's insurance.");

        var provider = await _providerRepository.GetByExternalIdAsync(
            request.ProviderId, cancellationToken);

        if (provider is null || provider.TenantId != tenantId)
            throw new ArgumentException(
                "Provider was not found in the current tenant.");

        claim.UpdateDetails(
            patient.Id,
            insurance.Id,
            payer.Id,
            provider.Id,
            claimType,
            request.ServiceFrom,
            request.ServiceTo,
            request.Notes);

        if (request.Lines is not null)
        {
            var replacementLines = BuildLines(
                tenantId, claim.Id, request.Lines);

            await _claimRepository.ReplaceLinesAsync(
                claim.Id, replacementLines, cancellationToken);

            claim.UpdateTotalChargeAmount(
                replacementLines.Sum(x => x.ChargeAmount));
        }

        await _claimRepository.UpdateAsync(claim, cancellationToken);
        await _claimRepository.SaveChangesAsync(cancellationToken);

        await AuditAsync(
            tenantId,
            "CLAIM_UPDATED",
            claim.ExternalId,
            cancellationToken);

        return true;
    }

    public async Task<ClaimValidationDto> ValidateAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var tenantId = RequireTenant();
        var claim = await FindClaimAsync(id, cancellationToken)
            ?? throw new KeyNotFoundException("Claim was not found.");

        var lines = await _claimRepository.GetLinesByClaimIdAsync(
            claim.Id, cancellationToken);

        var validation = BuildValidation(claim, lines);

        if (claim.Status == "DRAFT" ||
     claim.Status == "VALIDATION_FAILED" ||
     claim.Status == "READY_TO_SUBMIT")
        {
            var previousStatus = claim.Status;

            var newStatus = validation.IsValid
                ? "READY_TO_SUBMIT"
                : "VALIDATION_FAILED";

            claim.ChangeStatus(newStatus);

            if (!string.Equals(
                    previousStatus,
                    newStatus,
                    StringComparison.Ordinal))
            {
                await _claimRepository.AddHistoryAsync(
                    new ClaimHistory(
                        tenantId,
                        claim.Id,
                        previousStatus,
                        newStatus,
                        validation.IsValid
                            ? "Claim validation completed successfully."
                            : string.Join("; ", validation.Errors),
                        DateTime.UtcNow),
                    cancellationToken);
            }

            await _claimRepository.UpdateAsync(claim, cancellationToken);
            await _claimRepository.SaveChangesAsync(cancellationToken);

            await AuditAsync(
                tenantId,
                validation.IsValid
                    ? "CLAIM_VALIDATED"
                    : "CLAIM_VALIDATION_FAILED",
                claim.ExternalId,
                cancellationToken,
                validation.IsValid
                    ? null
                    : string.Join("; ", validation.Errors));
        }

        return validation;
    }

    public async Task<ClaimValidationDto?> GetValidationAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        RequireTenant();

        var claim = await FindClaimAsync(id, cancellationToken);

        if (claim is null)
            return null;

        var lines = await _claimRepository.GetLinesByClaimIdAsync(
            claim.Id, cancellationToken);

        // Validation is recalculated from current data. No validation-history
        // table or persisted validation snapshot exists in the supplied model.
        return BuildValidation(claim, lines);
    }

    public async Task<ClaimDto> SubmitAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        RequireTenant();

        var claim = await FindClaimAsync(id, cancellationToken)
            ?? throw new KeyNotFoundException("Claim was not found.");

        if (claim.Status != "READY_TO_SUBMIT")
            throw new InvalidOperationException(
                "Validate the claim successfully before submitting it.");

        // Never mark the claim as submitted without a real clearinghouse or
        // payer integration and a confirmed submission result.
        throw new NotSupportedException(
            "Claim submission is not configured. Connect a payer or " +
            "clearinghouse integration before submitting claims.");
    }

    public async Task<ClaimDto> ResubmitAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        RequireTenant();

        var claim = await FindClaimAsync(id, cancellationToken)
            ?? throw new KeyNotFoundException("Claim was not found.");

        if (claim.Status != "REJECTED")
        {
            throw new InvalidOperationException(
                "Only rejected claims can be resubmitted.");
        }

        // Do not mark the claim as resubmitted or submitted until a real
        // payer/clearinghouse integration confirms the submission.
        throw new NotSupportedException(
            "Claim resubmission is not configured. Connect a payer or " +
            "clearinghouse integration before resubmitting claims.");
    }

    public async Task<IReadOnlyList<ClaimRejectionDto>?> GetRejectionsAsync(
     Guid id,
     CancellationToken cancellationToken = default)
    {
        RequireTenant();

        var claim = await FindClaimAsync(id, cancellationToken);

        if (claim is null)
            return null;

        var rejections = await _claimRepository.GetRejectionsByClaimIdAsync(
            claim.Id,
            cancellationToken);

        return rejections
            .Select(rejection => new ClaimRejectionDto
            {
                Id = rejection.Id,
                ClaimId = claim.ExternalId,
                RejectionCode = rejection.RejectionCode,
                RejectionMessage = rejection.Description,
                Source = rejection.RejectionCategory,
                ExternalReference = null,
                RejectedAt = rejection.RejectedAt
            })
            .ToList();
    }
    public async Task<IReadOnlyList<ClaimHistoryDto>?> GetHistoryAsync(
        Guid claimId,
        CancellationToken cancellationToken = default)
    {
        RequireTenant();

        var claim = await FindClaimAsync(
            claimId,
            cancellationToken);

        if (claim is null)
        {
            return null;
        }

        var history = await _claimRepository.GetHistoryAsync(
            claim.Id,
            cancellationToken);

        return history
            .Select(x => new ClaimHistoryDto(
                x.Id,
                x.FromStatus,
                x.ToStatus,
                x.Note,
                x.ChangedAt,
                x.ChangedByUserId))
            .ToList();
    }
    public async Task<ClaimSubmissionResponseDto?> GetSubmissionResponseAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        RequireTenant();

        var claim = await FindClaimAsync(id, cancellationToken);

        if (claim is null)
            return null;

        if (claim.SubmittedAtUtc is null &&
            string.IsNullOrWhiteSpace(claim.SubmissionReference) &&
            string.IsNullOrWhiteSpace(claim.PayerClaimNumber))
        {
            return null;
        }

        return new ClaimSubmissionResponseDto(
            claim.ExternalId,
            claim.Status,
            claim.PayerClaimNumber,
            claim.SubmissionReference,
            claim.SubmittedAtUtc,
            claim.LastStatusCheckedAtUtc);
    }

    public async Task<IReadOnlyList<ClaimLineDto>?> GetLinesAsync(
        Guid claimId,
        CancellationToken cancellationToken = default)
    {
        RequireTenant();

        var claim = await FindClaimAsync(claimId, cancellationToken);

        if (claim is null)
            return null;

        var lines = await _claimRepository.GetLinesByClaimIdAsync(
            claim.Id, cancellationToken);

        return lines.Select(MapLineToDto).ToList();
    }

    public async Task<ClaimLineDto?> AddLineAsync(
        Guid claimId,
        ClaimLineInput request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var tenantId = RequireTenant();
        var claim = await FindClaimAsync(claimId, cancellationToken);

        if (claim is null)
            return null;

        EnsureDraft(claim);

        ValidateLine(request, claim.ServiceFrom, claim.ServiceTo);

        var existing = await _claimRepository.GetLinesByClaimIdAsync(
            claim.Id, cancellationToken);

        if (existing.Any(x => x.LineNumber == request.LineNumber))
            throw new ArgumentException(
                "A claim line with this line number already exists.");

        var line = BuildLine(tenantId, claim.Id, request);

        await _claimRepository.AddLineAsync(line, cancellationToken);

        claim.UpdateTotalChargeAmount(
            existing.Sum(x => x.ChargeAmount) + line.ChargeAmount);

        await _claimRepository.UpdateAsync(claim, cancellationToken);
        await _claimRepository.SaveChangesAsync(cancellationToken);

        await AuditAsync(
            tenantId,
            "CLAIM_LINE_CREATED",
            line.ExternalId,
            cancellationToken);

        return MapLineToDto(line);
    }

    public async Task<ClaimLineDto?> UpdateLineAsync(
        Guid claimId,
        Guid lineId,
        ClaimLineInput request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var tenantId = RequireTenant();
        var claim = await FindClaimAsync(claimId, cancellationToken);

        if (claim is null)
            return null;

        EnsureDraft(claim);
        ValidateLine(request, claim.ServiceFrom, claim.ServiceTo);

        var line = await _claimRepository.GetLineByExternalIdAsync(
            claim.Id, lineId, cancellationToken);

        if (line is null)
            return null;

        var existing = await _claimRepository.GetLinesByClaimIdAsync(
            claim.Id, cancellationToken);

        if (existing.Any(x =>
                x.Id != line.Id && x.LineNumber == request.LineNumber))
        {
            throw new ArgumentException(
                "A claim line with this line number already exists.");
        }

        // Check the immutable line number before changing any entity properties.
        if (line.LineNumber != request.LineNumber)
        {
            throw new InvalidOperationException(
                "Changing a claim line number is not supported by the current domain entity.");
        }

        line.UpdateDetails(
            request.ProcedureCode,
            request.Units,
            request.ChargeAmount,
            request.ServiceDate,
            request.Modifier1,
            request.Modifier2,
            request.Modifier3,
            request.Modifier4,
            request.PlaceOfService,
            request.DiagnosisPointers);

        // LineNumber is immutable in the supplied domain entity. Reject a
        // request to change it rather than silently ignoring the requested value.
        if (line.LineNumber != request.LineNumber)
            throw new InvalidOperationException(
                "Changing a claim line number is not supported by the current domain entity.");

        await _claimRepository.UpdateLineAsync(line, cancellationToken);

        var total = existing
            .Where(x => x.Id != line.Id)
            .Sum(x => x.ChargeAmount) + line.ChargeAmount;

        claim.UpdateTotalChargeAmount(total);

        await _claimRepository.UpdateAsync(claim, cancellationToken);
        await _claimRepository.SaveChangesAsync(cancellationToken);

        await AuditAsync(
            tenantId,
            "CLAIM_LINE_UPDATED",
            line.ExternalId,
            cancellationToken);

        return MapLineToDto(line);
    }

    public async Task<bool> DeleteLineAsync(
        Guid claimId,
        Guid lineId,
        CancellationToken cancellationToken = default)
    {
        var tenantId = RequireTenant();
        var claim = await FindClaimAsync(claimId, cancellationToken);

        if (claim is null)
            return false;

        EnsureDraft(claim);

        var line = await _claimRepository.GetLineByExternalIdAsync(
            claim.Id, lineId, cancellationToken);

        if (line is null)
            return false;

        await _claimRepository.DeleteLineAsync(line, cancellationToken);

        var remainingLines = await _claimRepository.GetLinesByClaimIdAsync(
            claim.Id, cancellationToken);

        claim.UpdateTotalChargeAmount(
            remainingLines
                .Where(x => x.Id != line.Id)
                .Sum(x => x.ChargeAmount));

        await _claimRepository.UpdateAsync(claim, cancellationToken);
        await _claimRepository.SaveChangesAsync(cancellationToken);

        await AuditAsync(
            tenantId,
            "CLAIM_LINE_DELETED",
            line.ExternalId,
            cancellationToken);

        return true;
    }

    // Claim Diagnoses

    public async Task<IReadOnlyList<ClaimDiagnosisDto>?> GetDiagnosesAsync(
        Guid claimId,
        CancellationToken cancellationToken = default)
    {
        RequireTenant();

        var claim = await FindClaimAsync(claimId, cancellationToken);

        if (claim is null)
            return null;

        var diagnoses = await _claimRepository.GetDiagnosesByClaimIdAsync(
            claim.Id,
            cancellationToken);

        return diagnoses
            .Select(x => new ClaimDiagnosisDto(
                x.Id,
                x.Icd10Code,
                x.Description,
                x.Sequence))
            .ToList();
    }

    public async Task<ClaimDiagnosisDto?> AddDiagnosisAsync(
        Guid claimId,
        ClaimDiagnosisInput request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var tenantId = RequireTenant();

        var claim = await FindClaimAsync(claimId, cancellationToken);

        if (claim is null)
            return null;

        EnsureDraft(claim);

        if (string.IsNullOrWhiteSpace(request.Icd10Code))
            throw new ArgumentException("ICD-10 diagnosis code is required.");

        if (request.Sequence < 1)
            throw new ArgumentOutOfRangeException(nameof(request.Sequence));

        var diagnoses = await _claimRepository.GetDiagnosesByClaimIdAsync(
            claim.Id,
            cancellationToken);

        if (diagnoses.Any(x => x.Sequence == request.Sequence))
            throw new ArgumentException(
                "A diagnosis with this sequence already exists.");

        var diagnosis = new ClaimDiagnosis(
            tenantId,
            claim.Id,
            request.Icd10Code,
            request.Description,
            request.Sequence);

        await _claimRepository.AddDiagnosisAsync(
            diagnosis,
            cancellationToken);

        await _claimRepository.SaveChangesAsync(cancellationToken);

        await AuditAsync(
            tenantId,
            "CLAIM_DIAGNOSIS_CREATED",
            claim.ExternalId,
            cancellationToken);

        return new ClaimDiagnosisDto(
            diagnosis.Id,
            diagnosis.Icd10Code,
            diagnosis.Description,
            diagnosis.Sequence);
    }

    public async Task<bool> DeleteDiagnosisAsync(
        Guid claimId,
        long diagnosisId,
        CancellationToken cancellationToken = default)
    {
        var tenantId = RequireTenant();

        var claim = await FindClaimAsync(claimId, cancellationToken);

        if (claim is null)
            return false;

        EnsureDraft(claim);

        var diagnoses = await _claimRepository.GetDiagnosesByClaimIdAsync(
            claim.Id,
            cancellationToken);

        var diagnosis = diagnoses.FirstOrDefault(x => x.Id == diagnosisId);

        if (diagnosis is null)
            return false;

        await _claimRepository.DeleteDiagnosisAsync(
            diagnosis,
            cancellationToken);

        await _claimRepository.SaveChangesAsync(cancellationToken);

        await AuditAsync(
            tenantId,
            "CLAIM_DIAGNOSIS_DELETED",
            claim.ExternalId,
            cancellationToken);

        return true;
    }

    // Claim Attachments

    public async Task<IReadOnlyList<ClaimAttachmentDto>?> GetAttachmentsAsync(
        Guid claimId,
        CancellationToken cancellationToken = default)
    {
        RequireTenant();

        var claim = await FindClaimAsync(claimId, cancellationToken);

        if (claim is null)
            return null;

        var attachments = await _claimRepository.GetAttachmentsByClaimIdAsync(
            claim.Id,
            cancellationToken);

        return attachments
            .Select(MapAttachmentToDto)
            .ToList();
    }

    public async Task<ClaimAttachmentDto?> AddAttachmentAsync(
        Guid claimId,
        ClaimAttachmentInput request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var tenantId = RequireTenant();

        var claim = await FindClaimAsync(claimId, cancellationToken);

        if (claim is null)
            return null;

        EnsureDraft(claim);

        if (string.IsNullOrWhiteSpace(request.FileName))
            throw new ArgumentException("File name is required.");

        if (string.IsNullOrWhiteSpace(request.StorageKey))
            throw new ArgumentException("Storage key is required.");

        if (request.FileSize < 0)
            throw new ArgumentOutOfRangeException(nameof(request.FileSize));

        var attachment = new ClaimAttachment(
            tenantId,
            claim.Id,
            request.FileName,
            request.StorageKey,
            request.ContentType,
            request.FileSize,
            request.DocumentType);

        await _claimRepository.AddAttachmentAsync(
            attachment,
            cancellationToken);

        await _claimRepository.SaveChangesAsync(cancellationToken);

        await AuditAsync(
            tenantId,
            "CLAIM_ATTACHMENT_CREATED",
            claim.ExternalId,
            cancellationToken);

        return MapAttachmentToDto(attachment);
    }

    public async Task<bool> DeleteAttachmentAsync(
        Guid claimId,
        long attachmentId,
        CancellationToken cancellationToken = default)
    {
        var tenantId = RequireTenant();

        var claim = await FindClaimAsync(claimId, cancellationToken);

        if (claim is null)
            return false;

        EnsureDraft(claim);

        var attachment = await _claimRepository.GetAttachmentByIdAsync(
            claim.Id,
            attachmentId,
            cancellationToken);

        if (attachment is null)
            return false;

        await _claimRepository.DeleteAttachmentAsync(
            attachment,
            cancellationToken);

        await _claimRepository.SaveChangesAsync(cancellationToken);

        await AuditAsync(
            tenantId,
            "CLAIM_ATTACHMENT_DELETED",
            claim.ExternalId,
            cancellationToken);

        return true;
    }

    private static ClaimAttachmentDto MapAttachmentToDto(
        ClaimAttachment attachment)
    {
        return new ClaimAttachmentDto(
            attachment.Id,
            attachment.FileName,
            attachment.ContentType,
            attachment.FileSize,
            attachment.DocumentType,
            attachment.UploadedAt);
    }

    private long RequireTenant()
    {
        if (!_tenantContext.TenantId.HasValue ||
            _tenantContext.TenantId.Value <= 0)
        {
            throw new InvalidOperationException(
                "An authenticated tenant context is required.");
        }

        return _tenantContext.TenantId.Value;
    }

    private async Task<Claim?> FindClaimAsync(
        Guid externalId,
        CancellationToken cancellationToken)
    {
        if (externalId == Guid.Empty)
            return null;

        var claim = await _claimRepository.GetByExternalIdAsync(
            externalId, cancellationToken);

        if (claim is null || claim.TenantId != RequireTenant())
            return null;

        return claim;
    }

    private async Task AuditAsync(
        long tenantId,
        string action,
        Guid entityId,
        CancellationToken cancellationToken,
        string? reason = null)
    {
        await _auditService.WriteAsync(
            new AuditEntry(
                tenantId,
                null,
                action,
                "Claim",
                entityId.ToString(),
                null,
                reason),
            cancellationToken);
    }

    private async Task<ClaimDto> MapToDtoAsync(
     Claim claim,
     IReadOnlyList<ClaimLine> lines,
     CancellationToken cancellationToken)
    {
        var relatedIds = await _claimRepository.GetRelatedExternalIdsAsync(
            claim.PatientId,
            claim.PatientInsuranceId,
            claim.PayerId,
            claim.ProviderId,
            cancellationToken);

        if (relatedIds is null)
        {
            throw new InvalidOperationException(
                $"One or more related records could not be found for claim {claim.ExternalId}.");
        }

        return new ClaimDto(
            claim.ExternalId,
            claim.TenantId,
            relatedIds.Value.PatientExternalId,
            relatedIds.Value.PatientInsuranceExternalId,
            relatedIds.Value.PayerExternalId,
            relatedIds.Value.ProviderExternalId,
            claim.ClaimNumber,
            claim.ClaimType,
            claim.Status,
            claim.ServiceFrom,
            claim.ServiceTo,
            claim.TotalChargeAmount,
            claim.PayerClaimNumber,
            claim.SubmissionReference,
            claim.SubmittedAtUtc,
            claim.LastStatusCheckedAtUtc,
            claim.Notes,
            lines.Select(MapLineToDto).ToList(),
            claim.CreatedAtUtc,
            claim.UpdatedAtUtc);
    }
    private static ClaimLineDto MapLineToDto(ClaimLine line)
    {
        return new ClaimLineDto(
            line.ExternalId,
            line.LineNumber,
            line.ProcedureCode,
            line.Modifier1,
            line.Modifier2,
            line.Modifier3,
            line.Modifier4,
            line.PlaceOfService,
            line.Units,
            line.ChargeAmount,
            line.ServiceDate,
            line.DiagnosisPointers);
    }

    private static List<ClaimLine> BuildLines(
        long tenantId,
        long claimId,
        IReadOnlyList<ClaimLineInput>? inputs)
    {
        var result = new List<ClaimLine>();

        if (inputs is null)
            return result;

        var duplicateLineNumber = inputs
            .GroupBy(x => x.LineNumber)
            .Any(group => group.Count() > 1);

        if (duplicateLineNumber)
            throw new ArgumentException(
                "Claim line numbers must be unique.");

        foreach (var input in inputs)
        {
            var line = BuildLine(tenantId, claimId, input);

            line.UpdateDetails(
                input.ProcedureCode,
                input.Units,
                input.ChargeAmount,
                input.ServiceDate,
                input.Modifier1,
                input.Modifier2,
                input.Modifier3,
                input.Modifier4,
                input.PlaceOfService,
                input.DiagnosisPointers);

            result.Add(line);
        }

        return result;
    }

    private static ClaimLine BuildLine(
        long tenantId,
        long claimId,
        ClaimLineInput input)
    {
        return new ClaimLine(
            tenantId,
            claimId,
            input.LineNumber,
            input.ProcedureCode,
            input.Units,
            input.ChargeAmount,
            input.ServiceDate);
    }

    private static ClaimValidationDto BuildValidation(
        Claim claim,
        IReadOnlyList<ClaimLine> lines)
    {
        var errors = new List<string>();
        var warnings = new List<string>();

        if (lines.Count == 0)
            errors.Add("At least one claim line is required.");

        if (claim.ServiceTo.Date < claim.ServiceFrom.Date)
            errors.Add("Service end date cannot be earlier than start date.");

        if (lines.Any(x =>
                x.ServiceDate.Date < claim.ServiceFrom.Date ||
                x.ServiceDate.Date > claim.ServiceTo.Date))
        {
            errors.Add(
                "Every claim line service date must be within the claim service date range.");
        }

        if (lines.Any(x => x.Units <= 0))
            errors.Add("All claim line units must be greater than zero.");

        if (lines.Any(x => x.ChargeAmount < 0))
            errors.Add("Claim line charges cannot be negative.");

        var duplicateNumbers = lines
            .GroupBy(x => x.LineNumber)
            .Any(group => group.Count() > 1);

        if (duplicateNumbers)
            errors.Add("Claim line numbers must be unique.");

        var calculatedTotal = decimal.Round(
            lines.Sum(x => x.ChargeAmount),
            2,
            MidpointRounding.AwayFromZero);

        if (calculatedTotal != claim.TotalChargeAmount)
        {
            warnings.Add(
                "The stored claim total differs from the calculated line total.");
        }

        return new ClaimValidationDto(
            claim.ExternalId,
            errors.Count == 0,
            errors,
            warnings,
            calculatedTotal,
            DateTime.UtcNow);
    }

    private static void ValidateLines(
        IReadOnlyList<ClaimLineInput>? lines,
        DateTime serviceFrom,
        DateTime serviceTo)
    {
        if (lines is null)
            return;

        if (lines.GroupBy(x => x.LineNumber).Any(g => g.Count() > 1))
            throw new ArgumentException(
                "Claim line numbers must be unique.");

        foreach (var line in lines)
            ValidateLine(line, serviceFrom, serviceTo);
    }

    private static void ValidateLine(
        ClaimLineInput line,
        DateTime serviceFrom,
        DateTime serviceTo)
    {
        if (line.LineNumber <= 0)
            throw new ArgumentOutOfRangeException(
                nameof(line.LineNumber));

        if (string.IsNullOrWhiteSpace(line.ProcedureCode))
            throw new ArgumentException("Procedure code is required.");

        if (line.Units <= 0)
            throw new ArgumentOutOfRangeException(nameof(line.Units));

        if (line.ChargeAmount < 0)
            throw new ArgumentOutOfRangeException(nameof(line.ChargeAmount));

        if (line.ServiceDate == default)
            throw new ArgumentException("Service date is required.");

        if (line.ServiceDate.Date < serviceFrom.Date ||
            line.ServiceDate.Date > serviceTo.Date)
        {
            throw new ArgumentException(
                "Claim line service date must be within the claim service date range.");
        }
    }

    private static void ValidateServiceDates(
        DateTime serviceFrom,
        DateTime serviceTo)
    {
        if (serviceFrom == default || serviceTo == default)
            throw new ArgumentException(
                "Claim service start and end dates are required.");

        if (serviceTo.Date < serviceFrom.Date)
            throw new ArgumentException(
                "Service end date cannot be earlier than start date.");
    }

    private static void EnsureDraft(Claim claim)
    {
        if (claim.Status != "DRAFT")
            throw new InvalidOperationException(
                "Only draft claims can be changed.");
    }

    private static void ValidateRequiredGuid(Guid value, string name)
    {
        if (value == Guid.Empty)
            throw new ArgumentException(
                "A valid external ID is required.", name);
    }

    private static string NormalizeRequired(
        string? value,
        string name)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException(
                $"{name} is required.", name);

        return value.Trim();
    }

    private static string? NormalizeOptional(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();


    // Claim status


    public async Task<IReadOnlyList<ClaimStatusDto>> GetPagedStatusesAsync(
        ClaimListRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var tenantId = RequireTenant();

        var page = Math.Max(1, request.Page);
        var pageSize = Math.Clamp(request.PageSize, 1, 100);

        long? patientId = null;
        long? payerId = null;

        if (request.PatientId.HasValue)
        {
            var patient = await _patientRepository.GetByExternalIdAsync(
                request.PatientId.Value,
                cancellationToken);

            if (patient is null || patient.TenantId != tenantId)
                return Array.Empty<ClaimStatusDto>();

            patientId = patient.Id;
        }

        if (request.PayerId.HasValue)
        {
            var payer = await _payerRepository.GetByExternalIdAsync(
                request.PayerId.Value,
                cancellationToken);

            if (payer is null || payer.TenantId != tenantId)
                return Array.Empty<ClaimStatusDto>();

            payerId = payer.Id;
        }

        var claims = await _claimRepository.GetPagedAsync(
            request.Search,
            NormalizeOptional(request.Status)?.ToUpperInvariant(),
            NormalizeOptional(request.ClaimType)?.ToUpperInvariant(),
            patientId,
            payerId,
            request.ServiceFrom,
            request.ServiceTo,
            page,
            pageSize,
            cancellationToken);

        return claims
            .Where(x => x.TenantId == tenantId)
            .Select(x => new ClaimStatusDto(
                x.ExternalId,
                x.ClaimNumber,
                x.Status,
                x.PayerClaimNumber,
                x.SubmissionReference,
                x.SubmittedAtUtc,
                x.LastStatusCheckedAtUtc,
                x.UpdatedAtUtc))
            .ToList();
    }


    public async Task<ClaimStatusDto?> GetStatusAsync(
        Guid claimId,
        CancellationToken cancellationToken = default)
    {
        var claim = await FindClaimAsync(claimId, cancellationToken);

        if (claim is null)
        {
            return null;
        }

        return new ClaimStatusDto(
            claim.ExternalId,
            claim.ClaimNumber,
            claim.Status,
            claim.PayerClaimNumber,
            claim.SubmissionReference,
            claim.SubmittedAtUtc,
            claim.LastStatusCheckedAtUtc,
            claim.UpdatedAtUtc);
    }

    public async Task<IReadOnlyList<ClaimStatusHistoryDto>?> GetStatusHistoryAsync(
        Guid claimId,
        CancellationToken cancellationToken = default)
    {
        var claim = await FindClaimAsync(claimId, cancellationToken);

        if (claim is null)
        {
            return null;
        }

        var history = await _claimRepository.GetHistoryAsync(
            claim.Id,
            cancellationToken);

        return history
            .Select(x => new ClaimStatusHistoryDto(
                x.Id,
                x.FromStatus,
                x.ToStatus,
                x.Note,
                x.ChangedAt,
                x.ChangedByUserId))
            .ToList();
    }

}
