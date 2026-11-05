using InsureFlow.Application.Persistence;
using InsureFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace InsureFlow.Infrastructure.Persistence.Repositories;

public sealed class InsurancePlanRepository
    : IInsurancePlanRepository
{
    private readonly InsureFlowDbContext _dbContext;

    public InsurancePlanRepository(
        InsureFlowDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<InsurancePlan>> GetPagedAsync(
        string? search,
        string? status,
        string? planType,
        string? networkType,
        long? payerId,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var query = _dbContext.InsurancePlans
            .Include(x => x.Payer)
            .AsNoTracking()
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var normalizedSearch = search.Trim();

            query = query.Where(x =>
                x.PlanCode.Contains(normalizedSearch) ||
                x.PlanName.Contains(normalizedSearch) ||
                x.Payer.Name.Contains(normalizedSearch));
        }

        if (!string.IsNullOrWhiteSpace(status))
        {
            var normalizedStatus = status.Trim();

            query = query.Where(x =>
                x.Status.ToString() == normalizedStatus);
        }

        if (!string.IsNullOrWhiteSpace(planType))
        {
            var normalizedPlanType = planType.Trim();

            query = query.Where(x =>
                x.PlanType.ToString() == normalizedPlanType);
        }

        if (!string.IsNullOrWhiteSpace(networkType))
        {
            var normalizedNetworkType =
                networkType.Trim();

            query = query.Where(x =>
                x.NetworkType.HasValue &&
                x.NetworkType.Value.ToString() ==
                normalizedNetworkType);
        }

        if (payerId.HasValue)
        {
            query = query.Where(x =>
                x.PayerId == payerId.Value);
        }

        return await query
            .OrderBy(x => x.PlanName)
            .ThenBy(x => x.PlanCode)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    public async Task<InsurancePlan?> GetByExternalIdAsync(
        Guid externalId,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.InsurancePlans
            .Include(x => x.Payer)
            .FirstOrDefaultAsync(
                x => x.ExternalId == externalId,
                cancellationToken);
    }

    public async Task<bool> ExistsByCodeAsync(
        string planCode,
        Guid? excludeExternalId = null,
        CancellationToken cancellationToken = default)
    {
        var query = _dbContext.InsurancePlans
            .Where(x => x.PlanCode == planCode);

        if (excludeExternalId.HasValue)
        {
            query = query.Where(
                x => x.ExternalId !=
                     excludeExternalId.Value);
        }

        return await query.AnyAsync(cancellationToken);
    }

    public async Task AddAsync(
        InsurancePlan insurancePlan,
        CancellationToken cancellationToken = default)
    {
        await _dbContext.InsurancePlans.AddAsync(
            insurancePlan,
            cancellationToken);
    }

    public Task UpdateAsync(
        InsurancePlan insurancePlan,
        CancellationToken cancellationToken = default)
    {
        _dbContext.InsurancePlans.Update(
            insurancePlan);

        return Task.CompletedTask;
    }

    public async Task SaveChangesAsync(
        CancellationToken cancellationToken = default)
    {
        await _dbContext.SaveChangesAsync(
            cancellationToken);
    }
}