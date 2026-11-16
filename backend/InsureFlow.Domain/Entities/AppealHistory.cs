
using InsureFlow.Domain.Common;

namespace InsureFlow.Domain.Entities;

public sealed class AppealHistory : AuditableEntity
{
    private AppealHistory()
    {
    }

    public AppealHistory(
        long tenantId,
        long appealId,
        string activityType,
        string description,
        long? performedByUserId = null)
    {
        if (tenantId <= 0)
            throw new ArgumentOutOfRangeException(nameof(tenantId));

        if (appealId <= 0)
            throw new ArgumentOutOfRangeException(nameof(appealId));

        if (string.IsNullOrWhiteSpace(activityType))
            throw new ArgumentException(
                "Activity type is required.",
                nameof(activityType));

        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException(
                "Activity description is required.",
                nameof(description));

        TenantId = tenantId;
        AppealId = appealId;
        ActivityType = activityType.Trim();
        Description = description.Trim();
        PerformedByUserId = performedByUserId;
        ActivityDateUtc = DateTime.UtcNow;
    }

    public long TenantId { get; private set; }

    public long AppealId { get; private set; }

    public string ActivityType { get; private set; } = string.Empty;

    public string Description { get; private set; } = string.Empty;

    public long? PerformedByUserId { get; private set; }

    public DateTime ActivityDateUtc { get; private set; }
}
