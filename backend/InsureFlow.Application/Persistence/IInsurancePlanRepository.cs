using InsureFlow.Domain.Entities;

namespace InsureFlow.Application.Persistence;

public interface IInsurancePlanRepository
{
    Task<IReadOnlyList<InsurancePlan>> GetPagedAsync(
        string? search,
        string? status,
        string? planType,
        string? networkType,
        long? payerId,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default);

    Task<InsurancePlan?> GetByExternalIdAsync(
        Guid externalId,
        CancellationToken cancellationToken = default);

    Task<bool> ExistsByCodeAsync(
        string planCode,
        Guid? excludeExternalId = null,
        CancellationToken cancellationToken = default);

    Task AddAsync(
        InsurancePlan insurancePlan,
        CancellationToken cancellationToken = default);

    Task UpdateAsync(
        InsurancePlan insurancePlan,
        CancellationToken cancellationToken = default);

    Task SaveChangesAsync(
        CancellationToken cancellationToken = default);
}