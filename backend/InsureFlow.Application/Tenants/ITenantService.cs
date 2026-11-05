using InsureFlow.Domain.Entities;

namespace InsureFlow.Application.Tenants;

public interface ITenantService
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

    Task CreateAsync(
        Tenant tenant,
        CancellationToken cancellationToken = default);

    Task UpdateAsync(
        Tenant tenant,
        CancellationToken cancellationToken = default);
}