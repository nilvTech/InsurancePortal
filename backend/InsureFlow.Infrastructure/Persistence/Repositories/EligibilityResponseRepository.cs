using InsureFlow.Application.Persistence;
using InsureFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace InsureFlow.Infrastructure.Persistence.Repositories;

public sealed class EligibilityResponseRepository(
    InsureFlowDbContext dbContext)
    : IEligibilityResponseRepository
{
    public async Task<EligibilityResponse?> GetByIdAsync(
        long id,
        bool includeBenefits = false,
        CancellationToken cancellationToken = default)
    {
        IQueryable<EligibilityResponse> query =
            dbContext.EligibilityResponses;

        if (includeBenefits)
        {
            query = query
                .Include(x => x.Benefits);
        }

        return await query
            .FirstOrDefaultAsync(
                x => x.Id == id,
                cancellationToken);
    }

    public async Task<EligibilityResponse?> GetByRequestIdAsync(
        long eligibilityRequestId,
        bool includeBenefits = false,
        CancellationToken cancellationToken = default)
    {
        IQueryable<EligibilityResponse> query =
            dbContext.EligibilityResponses;

        if (includeBenefits)
        {
            query = query
                .Include(x => x.Benefits);
        }

        return await query
            .FirstOrDefaultAsync(
                x => x.EligibilityRequestId ==
                     eligibilityRequestId,
                cancellationToken);
    }

    public async Task<bool> ExistsByRequestIdAsync(
        long eligibilityRequestId,
        CancellationToken cancellationToken = default)
    {
        return await dbContext.EligibilityResponses
            .AnyAsync(
                x => x.EligibilityRequestId ==
                     eligibilityRequestId,
                cancellationToken);
    }

    public async Task AddAsync(
        EligibilityResponse entity,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entity);

        await dbContext.EligibilityResponses
            .AddAsync(
                entity,
                cancellationToken);
    }

    public void Update(
        EligibilityResponse entity)
    {
        ArgumentNullException.ThrowIfNull(entity);

        dbContext.EligibilityResponses.Update(entity);
    }

    public void Remove(
        EligibilityResponse entity)
    {
        ArgumentNullException.ThrowIfNull(entity);

        dbContext.EligibilityResponses.Remove(entity);
    }

    public Task<int> SaveChangesAsync(
        CancellationToken cancellationToken = default)
    {
        return dbContext.SaveChangesAsync(
            cancellationToken);
    }
}