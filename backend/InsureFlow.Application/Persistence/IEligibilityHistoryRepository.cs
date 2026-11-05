using InsureFlow.Domain.Entities;

namespace InsureFlow.Application.Persistence;

public interface IEligibilityHistoryRepository
{
    Task<EligibilityHistory?> GetByIdAsync(
        long id,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<EligibilityHistory>> GetByRequestIdAsync(
        long eligibilityRequestId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<EligibilityHistory>> GetPagedAsync(
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default);

    Task AddAsync(
        EligibilityHistory entity,
        CancellationToken cancellationToken = default);

    Task AddRangeAsync(
        IEnumerable<EligibilityHistory> entities,
        CancellationToken cancellationToken = default);

    Task<int> SaveChangesAsync(
        CancellationToken cancellationToken = default);
}