
using InsureFlow.Domain.Common;

namespace InsureFlow.Domain.Entities;

public sealed class RolePermission : AuditableEntity
{
    private RolePermission() { }

    public RolePermission(long tenantId, long roleId, long permissionId)
    {
        TenantId = tenantId;
        RoleId = roleId;
        PermissionId = permissionId;
    }

    public long TenantId { get; private set; }
    public long RoleId { get; private set; }
    public long PermissionId { get; private set; }

    public Role Role { get; private set; } = null!;
    public Permission Permission { get; private set; } = null!;
}
