using InsureFlow.Application.Persistence;
using InsureFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace InsureFlow.Infrastructure.Persistence.Repositories;

public sealed class ProviderRepository : IProviderRepository
{
    private readonly InsureFlowDbContext _dbContext;

    public ProviderRepository(
        InsureFlowDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<Provider>> GetPagedAsync(
        string? search,
        string? status,
        long? organizationId,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var query = _dbContext.Providers
            .Include(x => x.Organization)
            .AsNoTracking()
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var normalizedSearch = search.Trim();

            query = query.Where(x =>
                x.Code.Contains(normalizedSearch) ||
                x.FirstName.Contains(normalizedSearch) ||
                (x.MiddleName != null &&
                 x.MiddleName.Contains(normalizedSearch)) ||
                x.LastName.Contains(normalizedSearch) ||
                x.Npi.Contains(normalizedSearch) ||
                (x.Specialty != null &&
                 x.Specialty.Contains(normalizedSearch)));
        }

        if (!string.IsNullOrWhiteSpace(status))
        {
            query = query.Where(x =>
                x.Status.ToString() == status.Trim());
        }

        if (organizationId.HasValue)
        {
            query = query.Where(x =>
                x.OrganizationId == organizationId.Value);
        }

        return await query
            .OrderBy(x => x.LastName)
            .ThenBy(x => x.FirstName)
            .ThenBy(x => x.Code)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    public async Task<Provider?> GetByExternalIdAsync(
        Guid externalId,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.Providers
            .Include(x => x.Organization)
            .FirstOrDefaultAsync(
                x => x.ExternalId == externalId,
                cancellationToken);
    }

    public async Task<Provider?> GetByIdAsync(
        long id,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.Providers
            .Include(x => x.Organization)
            .FirstOrDefaultAsync(
                x => x.Id == id,
                cancellationToken);
    }

    public async Task<bool> ExistsByCodeAsync(
        string code,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.Providers
            .AnyAsync(
                x => x.Code == code,
                cancellationToken);
    }

    public async Task<bool> ExistsByNpiAsync(
        string npi,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.Providers
            .AnyAsync(
                x => x.Npi == npi,
                cancellationToken);
    }

    public async Task AddAsync(
        Provider provider,
        CancellationToken cancellationToken = default)
    {
        await _dbContext.Providers.AddAsync(
            provider,
            cancellationToken);
    }

    public Task UpdateAsync(
        Provider provider,
        CancellationToken cancellationToken = default)
    {
        _dbContext.Providers.Update(provider);

        return Task.CompletedTask;
    }

    public async Task SaveChangesAsync(
        CancellationToken cancellationToken = default)
    {
        await _dbContext.SaveChangesAsync(
            cancellationToken);
    }
}