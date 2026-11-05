using InsureFlow.Domain.Entities;

namespace InsureFlow.Application.Persistence;

public interface IEligibilityResponseRepository
{
    Task<EligibilityResponse?> GetByIdAsync(
        long id,
        bool includeBenefits = false,
        CancellationToken cancellationToken = default);

    Task<EligibilityResponse?> GetByRequestIdAsync(
        long eligibilityRequestId,
        bool includeBenefits = false,
        CancellationToken cancellationToken = default);

    Task<bool> ExistsByRequestIdAsync(
        long eligibilityRequestId,
        CancellationToken cancellationToken = default);

    Task AddAsync(
        EligibilityResponse entity,
        CancellationToken cancellationToken = default);

    void Update(
        EligibilityResponse entity);

    void Remove(
        EligibilityResponse entity);

    Task<int> SaveChangesAsync(
        CancellationToken cancellationToken = default);
}