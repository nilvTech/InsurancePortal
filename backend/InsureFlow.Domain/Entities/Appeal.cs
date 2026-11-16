
using InsureFlow.Domain.Common;

namespace InsureFlow.Domain.Entities;

public sealed class Appeal : AuditableEntity
{
    private Appeal()
    {
    }

    public Appeal(
        long tenantId,
        long denialId,
        string appealNumber,
        string appealLevel,
        string appealReason,
        string? appealNotes = null,
        DateTime? appealDeadlineUtc = null)
    {
        if (tenantId <= 0)
            throw new ArgumentOutOfRangeException(nameof(tenantId));

        if (denialId <= 0)
            throw new ArgumentOutOfRangeException(nameof(denialId));

        if (string.IsNullOrWhiteSpace(appealNumber))
            throw new ArgumentException(
                "Appeal number is required.",
                nameof(appealNumber));

        if (string.IsNullOrWhiteSpace(appealLevel))
            throw new ArgumentException(
                "Appeal level is required.",
                nameof(appealLevel));

        if (string.IsNullOrWhiteSpace(appealReason))
            throw new ArgumentException(
                "Appeal reason is required.",
                nameof(appealReason));

        TenantId = tenantId;
        DenialId = denialId;
        AppealNumber = appealNumber.Trim();
        AppealLevel = appealLevel.Trim();
        AppealReason = appealReason.Trim();
        AppealNotes = NormalizeOptionalText(appealNotes);
        AppealDeadlineUtc = appealDeadlineUtc;
        Status = "DRAFT";
    }

    public long TenantId { get; private set; }

    public long DenialId { get; private set; }

    public string AppealNumber { get; private set; } = string.Empty;

    public string AppealLevel { get; private set; } = string.Empty;

    public string AppealReason { get; private set; } = string.Empty;

    public string? AppealNotes { get; private set; }

    public string Status { get; private set; } = "DRAFT";

    public DateTime? AppealDeadlineUtc { get; private set; }

    public DateTime? SubmittedAtUtc { get; private set; }

    public DateTime? WithdrawnAtUtc { get; private set; }

    public string? Decision { get; private set; }

    public string? DecisionNotes { get; private set; }

    public DateTime? DecisionAtUtc { get; private set; }

    public void UpdateDetails(
        string appealLevel,
        string appealReason,
        string? appealNotes,
        DateTime? appealDeadlineUtc)
    {
        EnsureDraft();

        if (string.IsNullOrWhiteSpace(appealLevel))
            throw new ArgumentException(
                "Appeal level is required.",
                nameof(appealLevel));

        if (string.IsNullOrWhiteSpace(appealReason))
            throw new ArgumentException(
                "Appeal reason is required.",
                nameof(appealReason));

        AppealLevel = appealLevel.Trim();
        AppealReason = appealReason.Trim();
        AppealNotes = NormalizeOptionalText(appealNotes);
        AppealDeadlineUtc = appealDeadlineUtc;

        SetUpdatedBy(null);
    }

    public void Submit()
    {
        EnsureDraft();

        Status = "SUBMITTED";
        SubmittedAtUtc = DateTime.UtcNow;

        SetUpdatedBy(null);
    }

    public void Withdraw()
    {
        if (Status != "SUBMITTED")
        {
            throw new InvalidOperationException(
                "Only submitted appeals can be withdrawn.");
        }

        Status = "WITHDRAWN";
        WithdrawnAtUtc = DateTime.UtcNow;

        SetUpdatedBy(null);
    }

    public void RecordDecision(
        string decision,
        string? decisionNotes)
    {
        if (Status != "SUBMITTED")
        {
            throw new InvalidOperationException(
                "A decision can only be recorded for a submitted appeal.");
        }

        if (string.IsNullOrWhiteSpace(decision))
        {
            throw new ArgumentException(
                "Decision is required.",
                nameof(decision));
        }

        var normalizedDecision = decision.Trim().ToUpperInvariant();

        if (normalizedDecision is not
            ("APPROVED" or "PARTIALLY_APPROVED" or "UPHELD"))
        {
            throw new ArgumentException(
                "Decision must be APPROVED, PARTIALLY_APPROVED, or UPHELD.",
                nameof(decision));
        }

        Decision = normalizedDecision;
        DecisionNotes = NormalizeOptionalText(decisionNotes);
        DecisionAtUtc = DateTime.UtcNow;
        Status = "DECIDED";

        SetUpdatedBy(null);
    }

    private void EnsureDraft()
    {
        if (Status != "DRAFT")
        {
            throw new InvalidOperationException(
                "Only draft appeals can be modified or submitted.");
        }
    }

    private static string? NormalizeOptionalText(string? value)
    {
        return string.IsNullOrWhiteSpace(value)
            ? null
            : value.Trim();
    }
}
