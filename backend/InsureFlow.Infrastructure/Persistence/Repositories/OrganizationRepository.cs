using InsureFlow.Application.Persistence;
using InsureFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace InsureFlow.Infrastructure.Persistence.Repositories;

public sealed class OrganizationRepository : IOrganizationRepository
{
    private readonly InsureFlowDbContext _dbContext;

    public OrganizationRepository(
        InsureFlowDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<Organization>> GetPagedAsync(
        string? search,
        string? status,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var query = _dbContext.Organizations
            .AsNoTracking()
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var normalizedSearch = search.Trim();

            query = query.Where(x =>
                x.Code.Contains(normalizedSearch) ||
                x.Name.Contains(normalizedSearch) ||
                (x.LegalName != null &&
                 x.LegalName.Contains(normalizedSearch)) ||
                (x.Npi != null &&
                 x.Npi.Contains(normalizedSearch)));
        }

        if (!string.IsNullOrWhiteSpace(status))
        {
            query = query.Where(x =>
                x.Status.ToString() == status.Trim());
        }

        return await query
            .OrderBy(x => x.Name)
            .ThenBy(x => x.Code)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    public async Task<Organization?> GetByExternalIdAsync(
        Guid externalId,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.Organizations
            .FirstOrDefaultAsync(
                x => x.ExternalId == externalId,
                cancellationToken);
    }

    public async Task<bool> ExistsByCodeAsync(
        string code,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.Organizations
            .AnyAsync(
                x => x.Code == code,
                cancellationToken);
    }

    public async Task AddAsync(
        Organization organization,
        CancellationToken cancellationToken = default)
    {
        await _dbContext.Organizations.AddAsync(
            organization,
            cancellationToken);
    }

    public Task UpdateAsync(
        Organization organization,
        CancellationToken cancellationToken = default)
    {
        _dbContext.Organizations.Update(organization);

        return Task.CompletedTask;
    }

    public async Task SaveChangesAsync(
        CancellationToken cancellationToken = default)
    {
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}