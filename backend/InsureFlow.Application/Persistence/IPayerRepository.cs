using InsureFlow.Domain.Entities;

namespace InsureFlow.Application.Persistence;

public interface IPayerRepository
{
    Task<IReadOnlyList<Payer>> GetPagedAsync(
        string? search,
        string? type,
        string? status,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default);

    Task<Payer?> GetByExternalIdAsync(
        Guid externalId,
        CancellationToken cancellationToken = default);

    Task<Payer?> GetByIdAsync(
        long id,
        CancellationToken cancellationToken = default);
    Task<bool> ExistsByCodeAsync(
        string code,
        Guid? excludeExternalId = null,
        CancellationToken cancellationToken = default);

    Task AddAsync(
        Payer payer,
        CancellationToken cancellationToken = default);

    Task UpdateAsync(
        Payer payer,
        CancellationToken cancellationToken = default);

    Task SaveChangesAsync(
        CancellationToken cancellationToken = default);
}

