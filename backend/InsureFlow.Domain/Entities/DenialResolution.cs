
using InsureFlow.Domain.Common;

namespace InsureFlow.Domain.Entities;

public sealed class DenialResolution : AuditableEntity
{
    public long TenantId { get; set; }

    public long DenialId { get; set; }

    public string ResolutionType { get; set; } = string.Empty;

    public string ResolutionNotes { get; set; } = string.Empty;

    public decimal ResolvedAmount { get; set; }

    public DateTime ResolutionDateUtc { get; set; }

    public long? ResolvedByUserId { get; set; }
}
