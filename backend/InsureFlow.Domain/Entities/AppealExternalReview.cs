
using InsureFlow.Domain.Common;

namespace InsureFlow.Domain.Entities;

public sealed class AppealExternalReview : AuditableEntity
{
    private AppealExternalReview()
    {
    }

    public AppealExternalReview(
        long tenantId,
        long appealId,
        string reviewingOrganization,
        string? externalReferenceNumber = null,
        DateTime? reviewDueAtUtc = null,
        string? reviewNotes = null)
    {
        if (tenantId <= 0)
            throw new ArgumentOutOfRangeException(nameof(tenantId));

        if (appealId <= 0)
            throw new ArgumentOutOfRangeException(nameof(appealId));

        if (string.IsNullOrWhiteSpace(reviewingOrganization))
            throw new ArgumentException(
                "Reviewing organization is required.",
                nameof(reviewingOrganization));

        TenantId = tenantId;
        AppealId = appealId;
        ReviewingOrganization = reviewingOrganization.Trim();
        ExternalReferenceNumber =
            NormalizeOptionalText(externalReferenceNumber);
        ReviewDueAtUtc = reviewDueAtUtc;
        ReviewNotes = NormalizeOptionalText(reviewNotes);
        Status = "REQUESTED";
        RequestedAtUtc = DateTime.UtcNow;
    }

    public long TenantId { get; private set; }

    public long AppealId { get; private set; }

    public string ReviewingOrganization { get; private set; } = string.Empty;

    public string? ExternalReferenceNumber { get; private set; }

    public string Status { get; private set; } = "REQUESTED";

    public DateTime RequestedAtUtc { get; private set; }

    public DateTime? ReviewDueAtUtc { get; private set; }

    public DateTime? RespondedAtUtc { get; private set; }

    public string? ReviewNotes { get; private set; }

    public void RecordResponse(
        string status,
        string? reviewNotes)
    {
        if (Status != "REQUESTED")
        {
            throw new InvalidOperationException(
                "A response can only be recorded for a requested external review.");
        }

        if (string.IsNullOrWhiteSpace(status))
        {
            throw new ArgumentException(
                "Review status is required.",
                nameof(status));
        }

        var normalizedStatus = status.Trim().ToUpperInvariant();

        if (normalizedStatus is not ("COMPLETED" or "DECLINED"))
        {
            throw new ArgumentException(
                "Review status must be COMPLETED or DECLINED.",
                nameof(status));
        }

        Status = normalizedStatus;
        ReviewNotes = NormalizeOptionalText(reviewNotes);
        RespondedAtUtc = DateTime.UtcNow;

        SetUpdatedBy(null);
    }

    private static string? NormalizeOptionalText(string? value)
    {
        return string.IsNullOrWhiteSpace(value)
            ? null
            : value.Trim();
    }
}
