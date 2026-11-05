using InsureFlow.Application.Persistence;
using InsureFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace InsureFlow.Infrastructure.Persistence.Repositories;

public sealed class EligibilityHistoryRepository(
    InsureFlowDbContext dbContext)
    : IEligibilityHistoryRepository
{
    public async Task<EligibilityHistory?> GetByIdAsync(
        long id,
        CancellationToken cancellationToken = default)
    {
        return await dbContext.EligibilityHistory
            .FirstOrDefaultAsync(
                x => x.Id == id,
                cancellationToken);
    }

    public async Task<IReadOnlyList<EligibilityHistory>>
        GetByRequestIdAsync(
            long eligibilityRequestId,
            CancellationToken cancellationToken = default)
    {
        return await dbContext.EligibilityHistory
            .AsNoTracking()
            .Where(
                x => x.EligibilityRequestId ==
                     eligibilityRequestId)
            .OrderByDescending(x => x.ChangedAt)
            .ThenByDescending(x => x.Id)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<EligibilityHistory>>
        GetPagedAsync(
            int pageNumber,
            int pageSize,
            CancellationToken cancellationToken = default)
    {
        if (pageNumber <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(pageNumber),
                "Page number must be greater than zero.");
        }

        if (pageSize <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(pageSize),
                "Page size must be greater than zero.");
        }

        return await dbContext.EligibilityHistory
            .AsNoTracking()
            .OrderByDescending(x => x.ChangedAt)
            .ThenByDescending(x => x.Id)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(
        EligibilityHistory entity,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entity);

        await dbContext.EligibilityHistory
            .AddAsync(
                entity,
                cancellationToken);
    }

    public async Task AddRangeAsync(
        IEnumerable<EligibilityHistory> entities,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entities);

        await dbContext.EligibilityHistory
            .AddRangeAsync(
                entities,
                cancellationToken);
    }

    public Task<int> SaveChangesAsync(
        CancellationToken cancellationToken = default)
    {
        return dbContext.SaveChangesAsync(
            cancellationToken);
    }
}