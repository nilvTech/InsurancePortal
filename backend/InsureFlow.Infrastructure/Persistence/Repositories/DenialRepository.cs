
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
}
