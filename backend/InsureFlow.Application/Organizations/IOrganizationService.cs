using InsureFlow.Contracts.Organizations;

namespace InsureFlow.Application.Organizations;

public interface IOrganizationService
{
    Task<IReadOnlyList<OrganizationDto>> GetPagedAsync(
        OrganizationListRequest request,
        CancellationToken cancellationToken = default);

    Task<OrganizationDto?> GetByIdAsync(
        Guid externalId,
        CancellationToken cancellationToken = default);

    Task<OrganizationDto> CreateAsync(
        CreateOrganizationRequest request,
        CancellationToken cancellationToken = default);

    Task<bool> UpdateAsync(
        Guid externalId,
        UpdateOrganizationRequest request,
        CancellationToken cancellationToken = default);

    Task<bool> DeactivateAsync(
        Guid externalId,
        CancellationToken cancellationToken = default);
}