using InsureFlow.Contracts.Providers;

namespace InsureFlow.Application.Providers;

public interface IProviderService
{
    Task<IReadOnlyList<ProviderDto>> GetPagedAsync(
        ProviderListRequest request,
        CancellationToken cancellationToken = default);

    Task<ProviderDto?> GetByIdAsync(
        Guid externalId,
        CancellationToken cancellationToken = default);

    Task<ProviderDto> CreateAsync(
        CreateProviderRequest request,
        CancellationToken cancellationToken = default);

    Task<bool> UpdateAsync(
        Guid externalId,
        UpdateProviderRequest request,
        CancellationToken cancellationToken = default);

    Task<bool> DeactivateAsync(
        Guid externalId,
        CancellationToken cancellationToken = default);
}