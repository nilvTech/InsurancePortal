
using InsureFlow.Contracts.Roles;

namespace InsureFlow.Application.Roles;

public interface IRoleService
{
    Task<IReadOnlyList<RoleResponse>> GetAllAsync(
        CancellationToken cancellationToken);

    Task<RoleResponse?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken);

    Task<RoleResponse> CreateAsync(
        CreateRoleRequest request,
        CancellationToken cancellationToken);

    Task<RoleResponse?> UpdateAsync(
        Guid id,
        UpdateRoleRequest request,
        CancellationToken cancellationToken);

    Task<bool> DeleteAsync(
        Guid id,
        CancellationToken cancellationToken);

    Task<IReadOnlyList<PermissionResponse>?> GetPermissionsAsync(
        Guid roleId,
        CancellationToken cancellationToken);

    Task<IReadOnlyList<PermissionResponse>?> UpdatePermissionsAsync(
        Guid roleId,
        UpdateRolePermissionsRequest request,
        CancellationToken cancellationToken);
}
