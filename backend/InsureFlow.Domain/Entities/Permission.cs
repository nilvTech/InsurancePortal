
using InsureFlow.Domain.Common;

namespace InsureFlow.Domain.Entities;

public sealed class Permission : AuditableEntity
{
    private Permission() { }

    public Permission(long tenantId, string code, string name, string? description)
    {
        if (tenantId <= 0)
            throw new ArgumentException("Tenant is required.");

        if (string.IsNullOrWhiteSpace(code))
            throw new ArgumentException("Permission code is required.");

        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Permission name is required.");

        TenantId = tenantId;
        ExternalId = Guid.NewGuid();
        Code = code.Trim().ToUpperInvariant();
        Name = name.Trim();
        Description = string.IsNullOrWhiteSpace(description)
            ? null
            : description.Trim();
        IsActive = true;
    }

    public Guid ExternalId { get; private set; }
    public long TenantId { get; private set; }
    public string Code { get; private set; } = null!;
    public string Name { get; private set; } = null!;
    public string? Description { get; private set; }
    public bool IsActive { get; private set; }
}
