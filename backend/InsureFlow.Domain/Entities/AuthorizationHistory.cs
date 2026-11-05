namespace InsureFlow.Domain.Entities;

public sealed class AuthorizationHistory
{
    private AuthorizationHistory() { }

    public AuthorizationHistory(
        long tenantId,
        long authorizationId,
        string fromStatus,
        string toStatus,
        string? note,
        DateTime changedAtUtc,
        long? changedByUserId = null)
    {
        if (tenantId <= 0)
            throw new ArgumentOutOfRangeException(nameof(tenantId));

        if (authorizationId <= 0)
            throw new ArgumentOutOfRangeException(nameof(authorizationId));

        TenantId = tenantId;
        AuthorizationId = authorizationId;

        FromStatus = fromStatus;
        ToStatus = toStatus;
        Note = note;

        ChangedAt = DateTime.SpecifyKind(
            changedAtUtc,
            DateTimeKind.Utc);

        ChangedByUserId = changedByUserId;
    }

    public long Id { get; private set; }

    public long TenantId { get; private set; }

    public long AuthorizationId { get; private set; }

    public string FromStatus { get; private set; } = null!;

    public string ToStatus { get; private set; } = null!;

    public string? Note { get; private set; }

    public DateTime ChangedAt { get; private set; }

    public long? ChangedByUserId { get; private set; }
}