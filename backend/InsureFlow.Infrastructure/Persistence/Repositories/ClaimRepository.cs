
using InsureFlow.Application.Persistence;
using InsureFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace InsureFlow.Infrastructure.Persistence.Repositories;

public sealed class ClaimRepository : IClaimRepository
{
    private readonly InsureFlowDbContext _dbContext;

    public ClaimRepository(InsureFlowDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    // Claims

    public async Task<IReadOnlyList<Claim>> GetPagedAsync(
        string? search,
        string? status,
        string? claimType,
        long? patientId,
        long? payerId,
        DateTime? serviceFrom,
        DateTime? serviceTo,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var query = _dbContext.Claims
            .AsNoTracking()
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var normalizedSearch = search.Trim();

            query = query.Where(x =>
                x.ClaimNumber.Contains(normalizedSearch) ||
                (x.PayerClaimNumber != null &&
                 x.PayerClaimNumber.Contains(normalizedSearch)));
        }

        if (!string.IsNullOrWhiteSpace(status))
        {
            var normalizedStatus = status.Trim();

            query = query.Where(x =>
                x.Status == normalizedStatus);
        }

        if (!string.IsNullOrWhiteSpace(claimType))
        {
            var normalizedClaimType = claimType.Trim();

            query = query.Where(x =>
                x.ClaimType == normalizedClaimType);
        }

        if (patientId.HasValue)
        {
            query = query.Where(x =>
                x.PatientId == patientId.Value);
        }

        if (payerId.HasValue)
        {
            query = query.Where(x =>
                x.PayerId == payerId.Value);
        }

        if (serviceFrom.HasValue)
        {
            query = query.Where(x =>
                x.ServiceTo >= serviceFrom.Value);
        }

        if (serviceTo.HasValue)
        {
            query = query.Where(x =>
                x.ServiceFrom <= serviceTo.Value);
        }

        return await query
            .OrderByDescending(x => x.CreatedAtUtc)
            .ThenBy(x => x.ClaimNumber)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    public async Task<Claim?> GetByExternalIdAsync(
        Guid externalId,
        CancellationToken cancellationToken = default)
    {
        if (externalId == Guid.Empty)
        {
            return null;
        }

        return await _dbContext.Claims
            .FirstOrDefaultAsync(
                x => x.ExternalId == externalId,
                cancellationToken);
    }

    public async Task<(Guid PatientExternalId,
                       Guid PatientInsuranceExternalId,
                       Guid PayerExternalId,
                       Guid ProviderExternalId)?> GetRelatedExternalIdsAsync(
        long patientId,
        long patientInsuranceId,
        long payerId,
        long providerId,
        CancellationToken cancellationToken = default)
    {
        var patientExternalId = await _dbContext.Patients
            .Where(x => x.Id == patientId)
            .Select(x => x.ExternalId)
            .FirstOrDefaultAsync(cancellationToken);

        var insuranceExternalId = await _dbContext.PatientInsurances
            .Where(x => x.Id == patientInsuranceId)
            .Select(x => x.ExternalId)
            .FirstOrDefaultAsync(cancellationToken);

        var payerExternalId = await _dbContext.Payers
            .Where(x => x.Id == payerId)
            .Select(x => x.ExternalId)
            .FirstOrDefaultAsync(cancellationToken);

        var providerExternalId = await _dbContext.Providers
            .Where(x => x.Id == providerId)
            .Select(x => x.ExternalId)
            .FirstOrDefaultAsync(cancellationToken);

        if (patientExternalId == Guid.Empty ||
            insuranceExternalId == Guid.Empty ||
            payerExternalId == Guid.Empty ||
            providerExternalId == Guid.Empty)
        {
            return null;
        }

        return (
            patientExternalId,
            insuranceExternalId,
            payerExternalId,
            providerExternalId);
    }

    public async Task AddAsync(
        Claim claim,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(claim);

        await _dbContext.Claims.AddAsync(
            claim,
            cancellationToken);
    }

    public Task UpdateAsync(
        Claim claim,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(claim);

        _dbContext.Claims.Update(claim);

        return Task.CompletedTask;
    }

    public async Task SaveChangesAsync(
        CancellationToken cancellationToken = default)
    {
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    // Claim Lines

    public async Task<IReadOnlyList<ClaimLine>> GetLinesByClaimIdAsync(
        long claimId,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.ClaimLines
            .AsNoTracking()
            .Where(x => x.ClaimId == claimId)
            .OrderBy(x => x.LineNumber)
            .ToListAsync(cancellationToken);
    }

    public async Task<ClaimLine?> GetLineByExternalIdAsync(
        long claimId,
        Guid lineExternalId,
        CancellationToken cancellationToken = default)
    {
        if (lineExternalId == Guid.Empty)
        {
            return null;
        }

        return await _dbContext.ClaimLines
            .FirstOrDefaultAsync(
                x => x.ClaimId == claimId &&
                     x.ExternalId == lineExternalId,
                cancellationToken);
    }

    public async Task AddLinesAsync(
        IReadOnlyCollection<ClaimLine> lines,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(lines);

        if (lines.Count == 0)
        {
            return;
        }

        await _dbContext.ClaimLines.AddRangeAsync(
            lines,
            cancellationToken);
    }

    public async Task AddLineAsync(
        ClaimLine line,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(line);

        await _dbContext.ClaimLines.AddAsync(
            line,
            cancellationToken);
    }

    public Task UpdateLineAsync(
        ClaimLine line,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(line);

        _dbContext.ClaimLines.Update(line);

        return Task.CompletedTask;
    }

    public Task DeleteLineAsync(
        ClaimLine line,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(line);

        _dbContext.ClaimLines.Remove(line);

        return Task.CompletedTask;
    }

    public async Task ReplaceLinesAsync(
        long claimId,
        IReadOnlyCollection<ClaimLine> lines,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(lines);

        var existingLines = await _dbContext.ClaimLines
            .Where(x => x.ClaimId == claimId)
            .ToListAsync(cancellationToken);

        _dbContext.ClaimLines.RemoveRange(existingLines);

        if (lines.Count > 0)
        {
            await _dbContext.ClaimLines.AddRangeAsync(
                lines,
                cancellationToken);
        }
    }

    // Claim History

    public async Task<IReadOnlyList<ClaimHistory>> GetHistoryAsync(
        long claimId,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.ClaimHistory
            .AsNoTracking()
            .Where(x => x.ClaimId == claimId)
            .OrderBy(x => x.ChangedAt)
            .ThenBy(x => x.Id)
            .ToListAsync(cancellationToken);
    }

    public async Task AddHistoryAsync(
        ClaimHistory history,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(history);

        await _dbContext.ClaimHistory.AddAsync(
            history,
            cancellationToken);
    }

    // Claim Rejections

    public async Task<IReadOnlyList<ClaimRejection>>
        GetRejectionsByClaimIdAsync(
            long claimId,
            CancellationToken cancellationToken = default)
    {
        return await _dbContext.ClaimRejections
            .AsNoTracking()
            .Where(x => x.ClaimId == claimId)
            .OrderByDescending(x => x.RejectedAt)
            .ThenByDescending(x => x.Id)
            .ToListAsync(cancellationToken);
    }

    public async Task<ClaimRejection?> GetRejectionByIdAsync(
        long claimId,
        long rejectionId,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.ClaimRejections
            .FirstOrDefaultAsync(
                x => x.ClaimId == claimId &&
                     x.Id == rejectionId,
                cancellationToken);
    }

    public async Task AddRejectionAsync(
        ClaimRejection rejection,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(rejection);

        await _dbContext.ClaimRejections.AddAsync(
            rejection,
            cancellationToken);
    }

    public Task UpdateRejectionAsync(
        ClaimRejection rejection,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(rejection);

        _dbContext.ClaimRejections.Update(rejection);

        return Task.CompletedTask;
    }

    // Claim Diagnoses

    public async Task<IReadOnlyList<ClaimDiagnosis>>
        GetDiagnosesByClaimIdAsync(
            long claimId,
            CancellationToken cancellationToken = default)
    {
        return await _dbContext.ClaimDiagnoses
            .AsNoTracking()
            .Where(x => x.ClaimId == claimId)
            .OrderBy(x => x.Sequence)
            .ThenBy(x => x.Id)
            .ToListAsync(cancellationToken);
    }

    public async Task AddDiagnosisAsync(
        ClaimDiagnosis diagnosis,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(diagnosis);

        await _dbContext.ClaimDiagnoses.AddAsync(
            diagnosis,
            cancellationToken);
    }

    public Task DeleteDiagnosisAsync(
        ClaimDiagnosis diagnosis,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(diagnosis);

        _dbContext.ClaimDiagnoses.Remove(diagnosis);

        return Task.CompletedTask;
    }

    // Claim Attachments

    public async Task<IReadOnlyList<ClaimAttachment>>
        GetAttachmentsByClaimIdAsync(
            long claimId,
            CancellationToken cancellationToken = default)
    {
        return await _dbContext.ClaimAttachments
            .AsNoTracking()
            .Where(x => x.ClaimId == claimId)
            .OrderByDescending(x => x.UploadedAt)
            .ThenByDescending(x => x.Id)
            .ToListAsync(cancellationToken);
    }

    public async Task<ClaimAttachment?> GetAttachmentByIdAsync(
        long claimId,
        long attachmentId,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.ClaimAttachments
            .FirstOrDefaultAsync(
                x => x.ClaimId == claimId &&
                     x.Id == attachmentId,
                cancellationToken);
    }

    public async Task AddAttachmentAsync(
        ClaimAttachment attachment,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(attachment);

        await _dbContext.ClaimAttachments.AddAsync(
            attachment,
            cancellationToken);
    }

    public Task DeleteAttachmentAsync(
        ClaimAttachment attachment,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(attachment);

        _dbContext.ClaimAttachments.Remove(attachment);

        return Task.CompletedTask;
    }
}
