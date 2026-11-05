namespace InsureFlow.Domain.Entities;

public sealed class ClaimHistory
{
    private ClaimHistory()
    {
    }

    public ClaimHistory(
        long tenantId,
        long claimId,
        string fromStatus,
        string toStatus,
        string? note,
        DateTime changedAtUtc,
        long? changedByUserId = null)
    {
        if (tenantId <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(tenantId));
        }

        if (claimId <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(claimId));
        }

        if (string.IsNullOrWhiteSpace(fromStatus))
        {
            throw new ArgumentException(
                "Previous claim status is required.",
                nameof(fromStatus));
        }

        if (string.IsNullOrWhiteSpace(toStatus))
        {
            throw new ArgumentException(
                "New claim status is required.",
                nameof(toStatus));
        }

        TenantId = tenantId;
        ClaimId = claimId;

        FromStatus = fromStatus.Trim();
        ToStatus = toStatus.Trim();

        Note = note;

        ChangedAt = DateTime.SpecifyKind(
            changedAtUtc,
            DateTimeKind.Utc);

        ChangedByUserId = changedByUserId;
    }

    public long Id { get; private set; }

    public long TenantId { get; private set; }

    public long ClaimId { get; private set; }

    public string FromStatus { get; private set; } = null!;

    public string ToStatus { get; private set; } = null!;

    public string? Note { get; private set; }

    public DateTime ChangedAt { get; private set; }

    public long? ChangedByUserId { get; private set; }
}