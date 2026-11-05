using InsureFlow.Domain.Common;
using InsureFlow.Domain.Enums;

namespace InsureFlow.Domain.Entities;

public sealed class EligibilityBenefit : BaseEntity
{
    private EligibilityBenefit()
    {
    }

    public EligibilityBenefit(
        long tenantId,
        long eligibilityResponseId,
        string category,
        string serviceTypeCode,
        string description,
        string? coverageLevel,
        decimal? copay,
        decimal? coinsurancePercent,
        decimal? deductible,
        decimal? remaining)
    {
        if (tenantId <= 0)
        {
            throw new ArgumentException(
                "Tenant ID is required.",
                nameof(tenantId));
        }

        if (eligibilityResponseId <= 0)
        {
            throw new ArgumentException(
                "Eligibility response ID is required.",
                nameof(eligibilityResponseId));
        }

        if (string.IsNullOrWhiteSpace(category))
        {
            throw new ArgumentException(
                "Benefit category is required.",
                nameof(category));
        }

        if (string.IsNullOrWhiteSpace(serviceTypeCode))
        {
            throw new ArgumentException(
                "Service type code is required.",
                nameof(serviceTypeCode));
        }

        if (string.IsNullOrWhiteSpace(description))
        {
            throw new ArgumentException(
                "Benefit description is required.",
                nameof(description));
        }

        ValidateMoney(copay, nameof(copay));
        ValidateMoney(deductible, nameof(deductible));
        ValidateMoney(remaining, nameof(remaining));

        if (coinsurancePercent.HasValue &&
            (coinsurancePercent.Value < 0 ||
             coinsurancePercent.Value > 100))
        {
            throw new ArgumentOutOfRangeException(
                nameof(coinsurancePercent));
        }

        TenantId = tenantId;
        EligibilityResponseId = eligibilityResponseId;
        Category = category.Trim();
        ServiceTypeCode = serviceTypeCode.Trim();
        Description = description.Trim();
        CoverageLevel = NormalizeOptional(coverageLevel);
        Copay = copay;
        CoinsurancePercent = coinsurancePercent;
        Deductible = deductible;
        Remaining = remaining;

        Status = EligibilityBenefitStatus.Active;
    }

    public long TenantId { get; private set; }

    public long EligibilityResponseId { get; private set; }

    public string Category { get; private set; } = null!;

    public string ServiceTypeCode { get; private set; } = null!;

    public string Description { get; private set; } = null!;

    public string? CoverageLevel { get; private set; }

    public decimal? Copay { get; private set; }

    public decimal? CoinsurancePercent { get; private set; }

    public decimal? Deductible { get; private set; }

    public decimal? Remaining { get; private set; }

    public EligibilityBenefitStatus Status { get; private set; }

    public EligibilityResponse EligibilityResponse { get; private set; }
        = null!;

    public void Update(
        string category,
        string serviceTypeCode,
        string description,
        string? coverageLevel,
        decimal? copay,
        decimal? coinsurancePercent,
        decimal? deductible,
        decimal? remaining)
    {
        if (string.IsNullOrWhiteSpace(category))
        {
            throw new ArgumentException(
                "Benefit category is required.",
                nameof(category));
        }

        if (string.IsNullOrWhiteSpace(serviceTypeCode))
        {
            throw new ArgumentException(
                "Service type code is required.",
                nameof(serviceTypeCode));
        }

        if (string.IsNullOrWhiteSpace(description))
        {
            throw new ArgumentException(
                "Benefit description is required.",
                nameof(description));
        }

        ValidateMoney(copay, nameof(copay));
        ValidateMoney(deductible, nameof(deductible));
        ValidateMoney(remaining, nameof(remaining));

        if (coinsurancePercent.HasValue &&
            (coinsurancePercent.Value < 0 ||
             coinsurancePercent.Value > 100))
        {
            throw new ArgumentOutOfRangeException(
                nameof(coinsurancePercent));
        }

        Category = category.Trim();
        ServiceTypeCode = serviceTypeCode.Trim();
        Description = description.Trim();
        CoverageLevel = NormalizeOptional(coverageLevel);
        Copay = copay;
        CoinsurancePercent = coinsurancePercent;
        Deductible = deductible;
        Remaining = remaining;
    }

    public void Activate()
    {
        Status = EligibilityBenefitStatus.Active;
    }

    public void Deactivate()
    {
        Status = EligibilityBenefitStatus.Inactive;
    }

    private static void ValidateMoney(
        decimal? value,
        string parameterName)
    {
        if (value.HasValue && value.Value < 0)
        {
            throw new ArgumentOutOfRangeException(
                parameterName,
                "Value cannot be negative.");
        }
    }

    private static string? NormalizeOptional(string? value)
    {
        return string.IsNullOrWhiteSpace(value)
            ? null
            : value.Trim();
    }
}