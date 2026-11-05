namespace InsureFlow.Domain.Entities;

public sealed class AuthorizationAppeal
{
    private AuthorizationAppeal() { }

    public AuthorizationAppeal(
        long tenantId,
        long authorizationId,
        string reason,
        string? notes,
        DateTime submittedAtUtc)
    {
        if (tenantId <= 0 || authorizationId <= 0)
            throw new ArgumentOutOfRangeException(nameof(tenantId));

        if (string.IsNullOrWhiteSpace(reason))
            throw new ArgumentException(
                "Appeal reason is required.",
                nameof(reason));

        TenantId = tenantId;
        AuthorizationId = authorizationId;
        Reason = reason.Trim();
        Notes = string.IsNullOrWhiteSpace(notes)
            ? null
            : notes.Trim();

        Status = "SUBMITTED";

        SubmittedAt = DateTime.SpecifyKind(
            submittedAtUtc,
            DateTimeKind.Utc);
    }

    public long Id { get; private set; }

    public long TenantId { get; private set; }

    public long AuthorizationId { get; private set; }

    public string Reason { get; private set; } = null!;

    public string? Notes { get; private set; }

    public string Status { get; private set; } = "SUBMITTED";

    public DateTime SubmittedAt { get; private set; }
}