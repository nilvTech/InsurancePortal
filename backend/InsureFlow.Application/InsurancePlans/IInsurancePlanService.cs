using InsureFlow.Contracts.InsurancePlans;

namespace InsureFlow.Application.InsurancePlans;

public interface IInsurancePlanService
{
    Task<IReadOnlyList<InsurancePlanDto>> GetPagedAsync(
        InsurancePlanListRequest request,
        CancellationToken cancellationToken = default);

    Task<InsurancePlanDto?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<InsurancePlanDto> CreateAsync(
        CreateInsurancePlanRequest request,
        CancellationToken cancellationToken = default);

    Task<bool> UpdateAsync(
        Guid id,
        UpdateInsurancePlanRequest request,
        CancellationToken cancellationToken = default);

    Task<bool> DeactivateAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<bool> ActivateAsync(
        Guid id,
        CancellationToken cancellationToken = default);
}