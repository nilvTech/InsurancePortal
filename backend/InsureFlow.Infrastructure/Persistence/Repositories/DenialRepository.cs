
using InsureFlow.Application.Persistence;
using InsureFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace InsureFlow.Infrastructure.Persistence.Repositories;

public sealed class DenialRepository : IDenialRepository
{
    private readonly InsureFlowDbContext _dbContext;

    public DenialRepository(InsureFlowDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<Denial>> GetPagedAsync(
        string? search,
        string? status,
        string? denialCategory,
        long? claimId,
        DateTime? denialDateFrom,
        DateTime? denialDateTo,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var query = _dbContext.Denials.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim();

            query = query.Where(x =>
                x.DenialNumber.Contains(term) ||
                x.DenialCode.Contains(term) ||
                x.DenialReason.Contains(term));
        }

        if (!string.IsNullOrWhiteSpace(status))
        {
            query = query.Where(x => x.Status == status.Trim());
        }

        if (!string.IsNullOrWhiteSpace(denialCategory))
        {
            query = query.Where(x =>
                x.DenialCategory == denialCategory.Trim());
        }

        if (claimId.HasValue)
        {
            query = query.Where(x => x.ClaimId == claimId.Value);
        }

        if (denialDateFrom.HasValue)
        {
            query = query.Where(x =>
                x.DenialDate >= denialDateFrom.Value);
        }

        if (denialDateTo.HasValue)
        {
            query = query.Where(x =>
                x.DenialDate <= denialDateTo.Value);
        }

        var skip = (page - 1) * pageSize;

        return await query
            .OrderByDescending(x => x.DenialDate)
            .ThenByDescending(x => x.Id)
            .Skip(skip)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    public Task<Denial?> GetByExternalIdAsync(
        Guid externalId,
        CancellationToken cancellationToken = default)
    {
        return _dbContext.Denials
            .FirstOrDefaultAsync(
                x => x.ExternalId == externalId,
                cancellationToken);
    }

    public Task<bool> ExistsByDenialNumberAsync(
        string denialNumber,
        CancellationToken cancellationToken = default)
    {
        return _dbContext.Denials
            .AnyAsync(
                x => x.DenialNumber == denialNumber,
                cancellationToken);
    }

    public async Task AddAsync(
        Denial denial,
        CancellationToken cancellationToken = default)
    {
        await _dbContext.Denials.AddAsync(
            denial,
            cancellationToken);
    }

    public void Update(Denial denial)
    {
        _dbContext.Denials.Update(denial);
    }

    public Task<int> SaveChangesAsync(
        CancellationToken cancellationToken = default)
    {
        return _dbContext.SaveChangesAsync(cancellationToken);
    }

    public Task<Guid?> GetClaimExternalIdAsync(
    long claimId,
    CancellationToken cancellationToken = default)
    {
        return _dbContext.Claims
            .Where(x => x.Id == claimId)
            .Select(x => (Guid?)x.ExternalId)
            .FirstOrDefaultAsync(cancellationToken);
    }


    public async Task<IReadOnlyList<DenialActivity>> GetActivitiesAsync(
        long denialId,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.DenialActivities
            .AsNoTracking()
            .Where(x => x.DenialId == denialId)
            .OrderByDescending(x => x.ActivityDateUtc)
            .ToListAsync(cancellationToken);
    }

    public async Task AddActivityAsync(
        DenialActivity activity,
        CancellationToken cancellationToken = default)
    {
        await _dbContext.DenialActivities.AddAsync(
            activity,
            cancellationToken);
    }
    // Denial Attachments

    public async Task<IReadOnlyList<DenialAttachment>> GetAttachmentsAsync(
        long denialId,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.DenialAttachments
            .AsNoTracking()
            .Where(x => x.DenialId == denialId)
            .OrderByDescending(x => x.UploadedAtUtc)
            .ToListAsync(cancellationToken);
    }

    public async Task<DenialAttachment?> GetAttachmentByExternalIdAsync(
        long denialId,
        Guid attachmentExternalId,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.DenialAttachments
            .FirstOrDefaultAsync(
                x => x.DenialId == denialId &&
                     x.ExternalId == attachmentExternalId,
                cancellationToken);
    }

    public async Task AddAttachmentAsync(
        DenialAttachment attachment,
        CancellationToken cancellationToken = default)
    {
        await _dbContext.DenialAttachments.AddAsync(
            attachment,
            cancellationToken);
    }

    public void RemoveAttachment(DenialAttachment attachment)
    {
        _dbContext.DenialAttachments.Remove(attachment);
    }

    // Denial Payer Responses

    public async Task<IReadOnlyList<DenialPayerResponse>> GetPayerResponsesAsync(
        long denialId,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.DenialPayerResponses
            .AsNoTracking()
            .Where(x => x.DenialId == denialId)
            .OrderByDescending(x => x.ResponseDateUtc)
            .ToListAsync(cancellationToken);
    }

    public async Task AddPayerResponseAsync(
        DenialPayerResponse response,
        CancellationToken cancellationToken = default)
    {
        await _dbContext.DenialPayerResponses.AddAsync(
            response,
            cancellationToken);
    }
}
