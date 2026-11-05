using InsureFlow.Domain.Entities;
using InsureFlow.Domain.Enums;

namespace InsureFlow.Application.Persistence;

public interface IEligibilityBenefitRepository
{
    Task<EligibilityBenefit?> GetByIdAsync(
        long id,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<EligibilityBenefit>> GetByResponseIdAsync(
        long eligibilityResponseId,
        bool activeOnly = false,
        CancellationToken cancellationToken = default);

    Task AddAsync(
        EligibilityBenefit entity,
        CancellationToken cancellationToken = default);

    Task AddRangeAsync(
        IEnumerable<EligibilityBenefit> entities,
        CancellationToken cancellationToken = default);

    void Update(
        EligibilityBenefit entity);

    void Remove(
        EligibilityBenefit entity);

    Task<int> SaveChangesAsync(
        CancellationToken cancellationToken = default);
}