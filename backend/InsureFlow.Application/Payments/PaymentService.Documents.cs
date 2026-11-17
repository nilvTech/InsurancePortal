
using InsureFlow.Application.Persistence;
using InsureFlow.Contracts.Payments;
using InsureFlow.Domain.Entities;

namespace InsureFlow.Application.Payments;

public sealed partial class PaymentService
{
    // 10. Get ERA records
    public async Task<EraListResponse> GetEraRecordsAsync(
        EraQueryRequest request,
        CancellationToken cancellationToken = default)
    {
        var records = await _repository.GetEraRecordsAsync(cancellationToken);

        var filtered = records.AsEnumerable();

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var search = request.Search.Trim();

            filtered = filtered.Where(x =>
                (x.TraceNumber?.Contains(search, StringComparison.OrdinalIgnoreCase) ?? false) ||
                (x.CheckNumber?.Contains(search, StringComparison.OrdinalIgnoreCase) ?? false) ||
                (x.FileName?.Contains(search, StringComparison.OrdinalIgnoreCase) ?? false));
        }

        if (!string.IsNullOrWhiteSpace(request.ImportStatus))
        {
            filtered = filtered.Where(x =>
                string.Equals(x.ImportStatus, request.ImportStatus,
                    StringComparison.OrdinalIgnoreCase));
        }

        if (request.PayerId.HasValue)
        {
            var payer = await _repository.GetPayerAsync(
                request.PayerId.Value, cancellationToken);

            filtered = payer == null
                ? Enumerable.Empty<EraRecord>()
                : filtered.Where(x => x.PayerId == payer.Id);
        }

        if (request.FromDateUtc.HasValue)
            filtered = filtered.Where(x =>
                x.ReceivedAtUtc >= request.FromDateUtc.Value);

        if (request.ToDateUtc.HasValue)
            filtered = filtered.Where(x =>
                x.ReceivedAtUtc <= request.ToDateUtc.Value);

        var totalCount = filtered.Count();
        var page = Math.Max(1, request.Page);
        var pageSize = Math.Clamp(request.PageSize, 1, 100);

        var items = filtered
            .OrderByDescending(x => x.ReceivedAtUtc)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        var responses = new List<EraResponse>();

        foreach (var record in items)
            responses.Add(await MapEraAsync(record, cancellationToken));

        return new EraListResponse(responses, page, pageSize, totalCount);
    }

    // 11. Get ERA record
    public async Task<EraResponse?> GetEraRecordAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var record = await _repository.GetEraRecordAsync(id, cancellationToken);

        return record == null
            ? null
            : await MapEraAsync(record, cancellationToken);
    }

    // 12. Import ERA record
    public async Task<EraResponse> ImportEraAsync(
        ImportEraRequest request,
        CancellationToken cancellationToken = default)
    {
        var payer = await _repository.GetPayerAsync(
            request.PayerId, cancellationToken)
            ?? throw new InvalidOperationException("Payer was not found.");

        var era = new EraRecord
        {
            TenantId = GetTenantId(),
            PayerId = payer.Id,
            TraceNumber = null,
            CheckNumber = null,
            TotalPaymentAmount = 0m,
            PaymentDateUtc = null,
            ReceivedAtUtc = DateTime.UtcNow,
            FileName = request.FileName,
            StorageReference = null,
            TransactionControlNumber = null,
            ImportStatus = "RECEIVED",
            ErrorMessage = null,
            RawContent = request.RawContent
        };

        await _repository.AddEraRecordAsync(era, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);

        return await MapEraAsync(era, cancellationToken);
    }

    // 13. Get EOB records
    public async Task<EobListResponse> GetEobRecordsAsync(
        EobQueryRequest request,
        CancellationToken cancellationToken = default)
    {
        var records = await _repository.GetEobRecordsAsync(cancellationToken);

        var filtered = records.AsEnumerable();

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var search = request.Search.Trim();

            filtered = filtered.Where(x =>
                (x.DocumentNumber?.Contains(search, StringComparison.OrdinalIgnoreCase) ?? false) ||
                (x.FileName?.Contains(search, StringComparison.OrdinalIgnoreCase) ?? false));
        }

        if (!string.IsNullOrWhiteSpace(request.Status))
        {
            filtered = filtered.Where(x =>
                string.Equals(x.Status, request.Status,
                    StringComparison.OrdinalIgnoreCase));
        }

        if (request.PayerId.HasValue)
        {
            var payer = await _repository.GetPayerAsync(
                request.PayerId.Value, cancellationToken);

            filtered = payer == null
                ? Enumerable.Empty<EobRecord>()
                : filtered.Where(x => x.PayerId == payer.Id);
        }

        if (request.ClaimId.HasValue)
        {
            var claim = await _repository.GetClaimAsync(
                request.ClaimId.Value, cancellationToken);

            filtered = claim == null
                ? Enumerable.Empty<EobRecord>()
                : filtered.Where(x => x.ClaimId == claim.Id);
        }

        if (request.FromDateUtc.HasValue)
            filtered = filtered.Where(x =>
                x.ReceivedAtUtc >= request.FromDateUtc.Value);

        if (request.ToDateUtc.HasValue)
            filtered = filtered.Where(x =>
                x.ReceivedAtUtc <= request.ToDateUtc.Value);

        var totalCount = filtered.Count();
        var page = Math.Max(1, request.Page);
        var pageSize = Math.Clamp(request.PageSize, 1, 100);

        var items = filtered
            .OrderByDescending(x => x.ReceivedAtUtc)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        var responses = new List<EobResponse>();

        foreach (var record in items)
            responses.Add(await MapEobAsync(record, cancellationToken));

        return new EobListResponse(responses, page, pageSize, totalCount);
    }

    // 14. Get EOB record
    public async Task<EobResponse?> GetEobRecordAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var record = await _repository.GetEobRecordAsync(id, cancellationToken);

        return record == null
            ? null
            : await MapEobAsync(record, cancellationToken);
    }

    // 15. Upload EOB metadata
    public async Task<EobResponse> UploadEobAsync(
        UploadEobRequest request,
        CancellationToken cancellationToken = default)
    {
        var tenantId = GetTenantId();

        Patient? patient = null;
        Claim? claim = null;

        if (request.PatientId.HasValue)
        {
            patient = await _repository.GetPatientAsync(
                request.PatientId.Value, cancellationToken)
                ?? throw new InvalidOperationException("Patient was not found.");
        }

        if (request.ClaimId.HasValue)
        {
            claim = await _repository.GetClaimAsync(
                request.ClaimId.Value, cancellationToken)
                ?? throw new InvalidOperationException("Claim was not found.");
        }

        var payer = await _repository.GetPayerAsync(
            request.PayerId, cancellationToken)
            ?? throw new InvalidOperationException("Payer was not found.");

        var eob = new EobRecord
        {
            TenantId = tenantId,
            PayerId = payer.Id,
            PatientId = patient?.Id,
            ClaimId = claim?.Id,
            DocumentNumber = request.DocumentNumber,
            FileName = request.FileName,
            ContentType = request.ContentType,
            StorageReference = request.StorageReference,
            FileSizeBytes = request.FileSizeBytes,
            ReceivedAtUtc = DateTime.UtcNow,
            StatementDateUtc = request.StatementDateUtc,
            Status = "RECEIVED",
            Notes = request.Notes
        };

        await _repository.AddEobRecordAsync(eob, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);

        return await MapEobAsync(eob, cancellationToken);
    }

    // ERA response mapping
    private async Task<EraResponse> MapEraAsync(
        EraRecord era,
        CancellationToken cancellationToken)
    {
        var payer = await _repository.GetPayerByIdAsync(
            era.PayerId, cancellationToken);

        return new EraResponse(
            era.ExternalId,
            payer?.ExternalId ?? Guid.Empty,
            era.TraceNumber,
            era.CheckNumber,
            era.TotalPaymentAmount,
            era.PaymentDateUtc,
            era.ReceivedAtUtc,
            era.FileName,
            era.StorageReference,
            era.TransactionControlNumber,
            era.ImportStatus,
            era.ErrorMessage);
    }

    // EOB response mapping
    private async Task<EobResponse> MapEobAsync(
        EobRecord eob,
        CancellationToken cancellationToken)
    {
        var payer = await _repository.GetPayerByIdAsync(
            eob.PayerId, cancellationToken);

        var patient = eob.PatientId.HasValue
            ? await _repository.GetPatientByIdAsync(
                eob.PatientId.Value, cancellationToken)
            : null;

        var claim = eob.ClaimId.HasValue
            ? await _repository.GetClaimByIdAsync(
                eob.ClaimId.Value, cancellationToken)
            : null;

        return new EobResponse(
            eob.ExternalId,
            payer?.ExternalId ?? Guid.Empty,
            patient?.ExternalId,
            claim?.ExternalId,
            eob.DocumentNumber,
            eob.FileName,
            eob.ContentType,
            eob.StorageReference,
            eob.FileSizeBytes,
            eob.ReceivedAtUtc,
            eob.StatementDateUtc,
            eob.Status,
            eob.Notes);
    }
}
