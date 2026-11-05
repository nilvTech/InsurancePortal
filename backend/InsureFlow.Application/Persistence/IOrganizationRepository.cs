using InsureFlow.Domain.Entities;

namespace InsureFlow.Application.Persistence;

public interface IOrganizationRepository
{
    Task<IReadOnlyList<Organization>> GetPagedAsync(
        string? search,
        string? status,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default);

    Task<Organization?> GetByExternalIdAsync(
        Guid externalId,
        CancellationToken cancellationToken = default);

    Task<bool> ExistsByCodeAsync(
        string code,
        CancellationToken cancellationToken = default);

    Task AddAsync(
        Organization organization,
        CancellationToken cancellationToken = default);

    Task UpdateAsync(
        Organization organization,
        CancellationToken cancellationToken = default);

    Task SaveChangesAsync(
        CancellationToken cancellationToken = default);
}