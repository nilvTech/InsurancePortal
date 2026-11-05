using InsureFlow.Application.Persistence;
using InsureFlow.Domain.Entities;

namespace InsureFlow.Application.Tenants;

public sealed class TenantService : ITenantService
{
    private readonly ITenantRepository _tenantRepository;

    public TenantService(ITenantRepository tenantRepository)
    {
        _tenantRepository = tenantRepository;
    }

    public async Task<Tenant?> GetByIdAsync(
        long tenantId,
        CancellationToken cancellationToken = default)
    {
        return await _tenantRepository.GetByIdAsync(
            tenantId,
            cancellationToken);
    }

    public async Task<Tenant?> GetByCodeAsync(
        string code,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(code))
        {
            throw new ArgumentException(
                "Tenant code is required.",
                nameof(code));
        }

        return await _tenantRepository.GetByCodeAsync(
            code,
            cancellationToken);
    }

    public async Task<bool> ExistsByCodeAsync(
        string code,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(code))
        {
            return false;
        }

        return await _tenantRepository.ExistsByCodeAsync(
            code,
            cancellationToken);
    }

    public async Task CreateAsync(
        Tenant tenant,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(tenant);

        var exists = await _tenantRepository.ExistsByCodeAsync(
            tenant.Code,
            cancellationToken);

        if (exists)
        {
            throw new InvalidOperationException(
                $"A tenant with code '{tenant.Code}' already exists.");
        }

        await _tenantRepository.AddAsync(
            tenant,
            cancellationToken);

        await _tenantRepository.SaveChangesAsync(
            cancellationToken);
    }

    public async Task UpdateAsync(
        Tenant tenant,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(tenant);

        await _tenantRepository.UpdateAsync(
            tenant,
            cancellationToken);

        await _tenantRepository.SaveChangesAsync(
            cancellationToken);
    }
}