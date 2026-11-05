using InsureFlow.Application.Persistence;
using InsureFlow.Domain.Entities;
using InsureFlow.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace InsureFlow.Infrastructure.Persistence.Repositories;

public sealed class EligibilityBenefitRepository(
    InsureFlowDbContext dbContext)
    : IEligibilityBenefitRepository
{
    public async Task<EligibilityBenefit?> GetByIdAsync(
        long id,
        CancellationToken cancellationToken = default)
    {
        return await dbContext.EligibilityBenefits
            .FirstOrDefaultAsync(
                x => x.Id == id,
                cancellationToken);
    }

    public async Task<IReadOnlyList<EligibilityBenefit>>
        GetByResponseIdAsync(
            long eligibilityResponseId,
            bool activeOnly = false,
            CancellationToken cancellationToken = default)
    {
        IQueryable<EligibilityBenefit> query =
            dbContext.EligibilityBenefits
                .AsNoTracking()
                .Where(
                    x => x.EligibilityResponseId ==
                         eligibilityResponseId);

        if (activeOnly)
        {
            query = query.Where(
                x => x.Status ==
                     EligibilityBenefitStatus.Active);
        }

        return await query
            .OrderBy(x => x.Category)
            .ThenBy(x => x.ServiceTypeCode)
            .ThenBy(x => x.Id)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(
        EligibilityBenefit entity,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entity);

        await dbContext.EligibilityBenefits
            .AddAsync(
                entity,
                cancellationToken);
    }

    public async Task AddRangeAsync(
        IEnumerable<EligibilityBenefit> entities,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entities);

        await dbContext.EligibilityBenefits
            .AddRangeAsync(
                entities,
                cancellationToken);
    }

    public void Update(
        EligibilityBenefit entity)
    {
        ArgumentNullException.ThrowIfNull(entity);

        dbContext.EligibilityBenefits.Update(entity);
    }

    public void Remove(
        EligibilityBenefit entity)
    {
        ArgumentNullException.ThrowIfNull(entity);

        dbContext.EligibilityBenefits.Remove(entity);
    }

    public Task<int> SaveChangesAsync(
        CancellationToken cancellationToken = default)
    {
        return dbContext.SaveChangesAsync(
            cancellationToken);
    }
}