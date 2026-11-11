
using InsureFlow.Domain.Common;

namespace InsureFlow.Domain.Entities;

public sealed class DenialActivity : AuditableEntity
{
    private DenialActivity()
    {
    }

    public DenialActivity(
        long tenantId,
        long denialId,
        string activityType,
        string description,
        long? performedByUserId = null)
    {
        if (tenantId <= 0)
            throw new ArgumentException("Tenant ID must be greater than zero.");

        if (denialId <= 0)
            throw new ArgumentException("Denial ID must be greater than zero.");

        if (string.IsNullOrWhiteSpace(activityType))
            throw new ArgumentException("Activity type is required.");

        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException("Description is required.");

        if (performedByUserId.HasValue && performedByUserId.Value <= 0)
            throw new ArgumentException("User ID must be greater than zero.");

        TenantId = tenantId;
        DenialId = denialId;
        ActivityType = activityType.Trim();
        Description = description.Trim();
        PerformedByUserId = performedByUserId;
        ActivityDateUtc = DateTime.UtcNow;
    }

    public long TenantId { get; private set; }

    public long DenialId { get; private set; }

    public string ActivityType { get; private set; } = string.Empty;

    public string Description { get; private set; } = string.Empty;

    public long? PerformedByUserId { get; private set; }

    public DateTime ActivityDateUtc { get; private set; }
}
