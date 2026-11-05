using InsureFlow.Domain.Common;

namespace InsureFlow.Domain.Entities;

public sealed class EligibilityHistory : BaseEntity
{
    private EligibilityHistory(long tenantId)
    {
        TenantId = tenantId;
    }

    public EligibilityHistory(
        long tenantId,
        long eligibilityRequestId,
        string fromStatus,
        string toStatus,
        string? reason,
        DateTime changedAtUtc,
        long? changedByUserId = null)
    {
        if (tenantId <= 0)
        {
            throw new ArgumentException(
                "Tenant ID is required.",
                nameof(tenantId));
        }

        if (eligibilityRequestId <= 0)
        {
            throw new ArgumentException(
                "Eligibility request ID is required.",
                nameof(eligibilityRequestId));
        }

        if (string.IsNullOrWhiteSpace(fromStatus))
        {
            throw new ArgumentException(
                "Previous status is required.",
                nameof(fromStatus));
        }

        if (string.IsNullOrWhiteSpace(toStatus))
        {
            throw new ArgumentException(
                "New status is required.",
                nameof(toStatus));
        }

        TenantId = tenantId;
        EligibilityRequestId = eligibilityRequestId;
        FromStatus = fromStatus.Trim();
        ToStatus = toStatus.Trim();
        Reason = NormalizeOptional(reason);
        ChangedAt = changedAtUtc;
        ChangedByUserId = changedByUserId;
    }

    public long TenantId { get; private set; }

    public long EligibilityRequestId { get; private set; }

    public string FromStatus { get; private set; } = null!;

    public string ToStatus { get; private set; } = null!;

    public string? Reason { get; private set; }

    public DateTime ChangedAt { get; private set; }

    public long? ChangedByUserId { get; private set; }

    public EligibilityRequest EligibilityRequest { get; private set; }
        = null!;

    private static string? NormalizeOptional(string? value)
    {
        return string.IsNullOrWhiteSpace(value)
            ? null
            : value.Trim();
    }
}