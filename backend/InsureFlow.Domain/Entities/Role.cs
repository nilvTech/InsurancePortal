
using InsureFlow.Domain.Common;

namespace InsureFlow.Domain.Entities;

public sealed class Role : AuditableEntity
{
    private Role() { }

    public Role(long tenantId, string name, string? description)
    {
        if (tenantId <= 0)
            throw new ArgumentException("Tenant is required.");

        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Role name is required.");

        TenantId = tenantId;
        ExternalId = Guid.NewGuid();
        Name = name.Trim();
        Description = Normalize(description);
        IsActive = true;
    }

    public Guid ExternalId { get; private set; }
    public long TenantId { get; private set; }
    public string Name { get; private set; } = null!;
    public string? Description { get; private set; }
    public bool IsActive { get; private set; }

    public ICollection<RolePermission> RolePermissions { get; private set; }
        = new List<RolePermission>();

    public void Update(string name, string? description)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Role name is required.");

        Name = name.Trim();
        Description = Normalize(description);
        UpdatedAtUtc = DateTime.UtcNow;
    }

    public void Deactivate()
    {
        IsActive = false;
        UpdatedAtUtc = DateTime.UtcNow;
    }

    private static string? Normalize(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
