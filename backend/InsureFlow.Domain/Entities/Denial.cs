
using InsureFlow.Domain.Common;

namespace InsureFlow.Domain.Entities;

public sealed class Denial : AuditableEntity
{
    private Denial()
    {
    }

    public Denial(
        long tenantId,
        long claimId,
        string denialNumber,
        string denialCode,
        string denialReason,
        string denialCategory,
        decimal deniedAmount,
        DateTime denialDate,
        DateTime? appealDeadline = null)
    {
        if (tenantId <= 0)
            throw new ArgumentException("Tenant ID must be greater than zero.", nameof(tenantId));

        if (claimId <= 0)
            throw new ArgumentException("Claim ID must be greater than zero.", nameof(claimId));

        if (string.IsNullOrWhiteSpace(denialNumber))
            throw new ArgumentException("Denial number is required.", nameof(denialNumber));

        if (string.IsNullOrWhiteSpace(denialCode))
            throw new ArgumentException("Denial code is required.", nameof(denialCode));

        if (string.IsNullOrWhiteSpace(denialReason))
            throw new ArgumentException("Denial reason is required.", nameof(denialReason));

        if (string.IsNullOrWhiteSpace(denialCategory))
            throw new ArgumentException("Denial category is required.", nameof(denialCategory));

        if (deniedAmount < 0)
            throw new ArgumentException("Denied amount cannot be negative.", nameof(deniedAmount));

        TenantId = tenantId;
        ClaimId = claimId;
        DenialNumber = denialNumber.Trim();
        DenialCode = denialCode.Trim();
        DenialReason = denialReason.Trim();
        DenialCategory = denialCategory.Trim();
        DeniedAmount = deniedAmount;
        DenialDate = denialDate;
        AppealDeadline = appealDeadline;
        Status = "OPEN";
    }

    public long TenantId { get; private set; }

    public long ClaimId { get; private set; }

    public string DenialNumber { get; private set; } = string.Empty;

    public string DenialCode { get; private set; } = string.Empty;

    public string DenialReason { get; private set; } = string.Empty;

    public string DenialCategory { get; private set; } = string.Empty;

    public decimal DeniedAmount { get; private set; }

    public DateTime DenialDate { get; private set; }

    public DateTime? AppealDeadline { get; private set; }

    public string Status { get; private set; } = "OPEN";

    public long? AssignedToUserId { get; private set; }

    public void UpdateDetails(
        string denialCode,
        string denialReason,
        string denialCategory,
        decimal deniedAmount,
        DateTime denialDate,
        DateTime? appealDeadline)
    {
        if (Status == "RESOLVED")
            throw new InvalidOperationException("A resolved denial cannot be updated.");

        if (string.IsNullOrWhiteSpace(denialCode))
            throw new ArgumentException("Denial code is required.", nameof(denialCode));

        if (string.IsNullOrWhiteSpace(denialReason))
            throw new ArgumentException("Denial reason is required.", nameof(denialReason));

        if (string.IsNullOrWhiteSpace(denialCategory))
            throw new ArgumentException("Denial category is required.", nameof(denialCategory));

        if (deniedAmount < 0)
            throw new ArgumentException("Denied amount cannot be negative.", nameof(deniedAmount));

        DenialCode = denialCode.Trim();
        DenialReason = denialReason.Trim();
        DenialCategory = denialCategory.Trim();
        DeniedAmount = deniedAmount;
        DenialDate = denialDate;
        AppealDeadline = appealDeadline;
    }

    public void Assign(long userId)
    {
        if (userId <= 0)
            throw new ArgumentException("User ID must be greater than zero.", nameof(userId));

        if (Status == "RESOLVED")
            throw new InvalidOperationException("A resolved denial cannot be assigned.");

        AssignedToUserId = userId;
    }

    public void Unassign()
    {
        AssignedToUserId = null;
    }

    public void MarkAppealed()
    {
        if (Status != "OPEN")
            throw new InvalidOperationException("Only open denials can be appealed.");

        Status = "APPEALED";
    }

    public void MarkResolved()
    {
        if (Status == "RESOLVED")
            throw new InvalidOperationException("Denial is already resolved.");

        Status = "RESOLVED";
    }
}
