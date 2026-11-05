using InsureFlow.Domain.Common;
using InsureFlow.Domain.Enums;

namespace InsureFlow.Domain.Entities;

public sealed class InsurancePlan : AuditableEntity
{
    private InsurancePlan()
    {
    }

    public InsurancePlan(
        long tenantId,
        long payerId,
        string planCode,
        string planName,
        InsurancePlanType planType,
        InsuranceNetworkType? networkType)
    {
        if (tenantId <= 0)
        {
            throw new ArgumentException(
                "Tenant ID is required.",
                nameof(tenantId));
        }

        if (payerId <= 0)
        {
            throw new ArgumentException(
                "Payer ID is required.",
                nameof(payerId));
        }

        if (string.IsNullOrWhiteSpace(planCode))
        {
            throw new ArgumentException(
                "Plan code is required.",
                nameof(planCode));
        }

        if (string.IsNullOrWhiteSpace(planName))
        {
            throw new ArgumentException(
                "Plan name is required.",
                nameof(planName));
        }

        TenantId = tenantId;
        PayerId = payerId;
        PlanCode = planCode.Trim().ToUpperInvariant();
        PlanName = planName.Trim();
        PlanType = planType;
        NetworkType = networkType;
        Status = RecordStatus.Active;
    }

    public long TenantId { get; private set; }

    public long PayerId { get; private set; }

    public string PlanCode { get; private set; } = null!;

    public string PlanName { get; private set; } = null!;

    public InsurancePlanType PlanType { get; private set; }

    public InsuranceNetworkType? NetworkType { get; private set; }

    public RecordStatus Status { get; private set; }

    public Tenant Tenant { get; private set; } = null!;

    public Payer Payer { get; private set; } = null!;

    public void UpdateDetails(
        long payerId,
        string planCode,
        string planName,
        InsurancePlanType planType,
        InsuranceNetworkType? networkType)
    {
        if (payerId <= 0)
        {
            throw new ArgumentException(
                "Payer ID is required.",
                nameof(payerId));
        }

        if (string.IsNullOrWhiteSpace(planCode))
        {
            throw new ArgumentException(
                "Plan code is required.",
                nameof(planCode));
        }

        if (string.IsNullOrWhiteSpace(planName))
        {
            throw new ArgumentException(
                "Plan name is required.",
                nameof(planName));
        }

        PayerId = payerId;
        PlanCode = planCode.Trim().ToUpperInvariant();
        PlanName = planName.Trim();
        PlanType = planType;
        NetworkType = networkType;
        UpdatedAtUtc = DateTime.UtcNow;
    }

    public void Activate()
    {
        Status = RecordStatus.Active;
        UpdatedAtUtc = DateTime.UtcNow;
    }

    public void Deactivate()
    {
        Status = RecordStatus.Inactive;
        UpdatedAtUtc = DateTime.UtcNow;
    }
}