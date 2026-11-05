using InsureFlow.Application.Persistence;
using InsureFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace InsureFlow.Infrastructure.Persistence.Repositories;

public sealed class TenantRepository : ITenantRepository
{
    private readonly InsureFlowDbContext _dbContext;

    public TenantRepository(InsureFlowDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Tenant?> GetByIdAsync(
        long tenantId,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.Tenants
            .FirstOrDefaultAsync(
                x => x.Id == tenantId,
                cancellationToken);
    }

    public async Task<Tenant?> GetByCodeAsync(
        string code,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.Tenants
            .FirstOrDefaultAsync(
                x => x.Code == code,
                cancellationToken);
    }

    public async Task<bool> ExistsByCodeAsync(
        string code,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.Tenants
            .AnyAsync(
                x => x.Code == code,
                cancellationToken);
    }

    public async Task AddAsync(
        Tenant tenant,
        CancellationToken cancellationToken = default)
    {
        await _dbContext.Tenants.AddAsync(
            tenant,
            cancellationToken);
    }

    public Task UpdateAsync(
        Tenant tenant,
        CancellationToken cancellationToken = default)
    {
        _dbContext.Tenants.Update(tenant);

        return Task.CompletedTask;
    }

    public async Task SaveChangesAsync(
        CancellationToken cancellationToken = default)
    {
        await _dbContext.SaveChangesAsync(
            cancellationToken);
    }
}