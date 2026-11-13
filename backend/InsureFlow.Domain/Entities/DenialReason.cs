
using InsureFlow.Domain.Common;

namespace InsureFlow.Domain.Entities;

public sealed class DenialReason : AuditableEntity
{
    public long TenantId { get; set; }

    public string ReasonCode { get; set; } = string.Empty;

    public string ReasonName { get; set; } = string.Empty;

    public string? Description { get; set; }

    public string Category { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;
}
