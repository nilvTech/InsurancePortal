using InsureFlow.Domain.Entities;

namespace InsureFlow.Application.Persistence;

public interface ITenantRepository
{
    Task<Tenant?> GetByIdAsync(
        long tenantId,
        CancellationToken cancellationToken = default);

    Task<Tenant?> GetByCodeAsync(
        string code,
        CancellationToken cancellationToken = default);

    Task<bool> ExistsByCodeAsync(
        string code,
        CancellationToken cancellationToken = default);

    Task AddAsync(
        Tenant tenant,
        CancellationToken cancellationToken = default);

    Task UpdateAsync(
        Tenant tenant,
        CancellationToken cancellationToken = default);

    Task SaveChangesAsync(
        CancellationToken cancellationToken = default);
}