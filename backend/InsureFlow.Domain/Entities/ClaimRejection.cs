namespace InsureFlow.Domain.Entities;

public sealed class ClaimRejection
{
    private ClaimRejection()
    {
    }

    public ClaimRejection(
        long tenantId,
        long claimId,
        string rejectionCode,
        string description,
        DateTime rejectedAtUtc,
        string? rejectionCategory = null)
    {
        if (tenantId <= 0)
            throw new ArgumentOutOfRangeException(nameof(tenantId));

        if (claimId <= 0)
            throw new ArgumentOutOfRangeException(nameof(claimId));

        if (string.IsNullOrWhiteSpace(rejectionCode))
            throw new ArgumentException(
                "Rejection code is required.",
                nameof(rejectionCode));

        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException(
                "Rejection description is required.",
                nameof(description));

        TenantId = tenantId;
        ClaimId = claimId;
        RejectionCode = rejectionCode.Trim();
        RejectionCategory = string.IsNullOrWhiteSpace(rejectionCategory)
            ? null
            : rejectionCategory.Trim();

        Description = description.Trim();
        Status = "OPEN";

        RejectedAt = DateTime.SpecifyKind(
            rejectedAtUtc,
            DateTimeKind.Utc);
    }

    public long Id { get; private set; }

    public long TenantId { get; private set; }

    public long ClaimId { get; private set; }

    public string RejectionCode { get; private set; } = string.Empty;

    public string? RejectionCategory { get; private set; }

    public string Description { get; private set; } = string.Empty;

    public string Status { get; private set; } = "OPEN";

    public DateTime RejectedAt { get; private set; }

    public DateTime? ResolvedAt { get; private set; }

    public string? ResolutionNote { get; private set; }

    public void Resolve(
        string resolutionNote,
        DateTime resolvedAtUtc)
    {
        if (Status == "RESOLVED")
            throw new InvalidOperationException(
                "This rejection has already been resolved.");

        if (string.IsNullOrWhiteSpace(resolutionNote))
            throw new ArgumentException(
                "Resolution note is required.",
                nameof(resolutionNote));

        Status = "RESOLVED";
        ResolutionNote = resolutionNote.Trim();

        ResolvedAt = DateTime.SpecifyKind(
            resolvedAtUtc,
            DateTimeKind.Utc);
    }
}