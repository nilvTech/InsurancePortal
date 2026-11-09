
using InsureFlow.Domain.Entities;

namespace InsureFlow.Application.Persistence;

public interface IDenialRepository
{
    Task<IReadOnlyList<Denial>> GetPagedAsync(
        string? search,
        string? status,
        string? denialCategory,
        long? claimId,
        DateTime? denialDateFrom,
        DateTime? denialDateTo,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default);

    Task<Denial?> GetByExternalIdAsync(
        Guid externalId,
        CancellationToken cancellationToken = default);

    Task<bool> ExistsByDenialNumberAsync(
        string denialNumber,
        CancellationToken cancellationToken = default);

    Task AddAsync(
        Denial denial,
        CancellationToken cancellationToken = default);

    void Update(Denial denial);

    Task<int> SaveChangesAsync(
        CancellationToken cancellationToken = default);
}
