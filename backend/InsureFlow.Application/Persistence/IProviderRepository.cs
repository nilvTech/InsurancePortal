using InsureFlow.Domain.Entities;

namespace InsureFlow.Application.Persistence;

public interface IProviderRepository
{
    Task<IReadOnlyList<Provider>> GetPagedAsync(
        string? search,
        string? status,
        long? organizationId,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default);

    Task<Provider?> GetByExternalIdAsync(
        Guid externalId,
        CancellationToken cancellationToken = default);

    Task<Provider?> GetByIdAsync(
        long id,
        CancellationToken cancellationToken = default);


    Task<bool> ExistsByCodeAsync(
        string code,
        CancellationToken cancellationToken = default);

    Task<bool> ExistsByNpiAsync(
        string npi,
        CancellationToken cancellationToken = default);

    Task AddAsync(
        Provider provider,
        CancellationToken cancellationToken = default);

    Task UpdateAsync(
        Provider provider,
        CancellationToken cancellationToken = default);

    Task SaveChangesAsync(
        CancellationToken cancellationToken = default);
}