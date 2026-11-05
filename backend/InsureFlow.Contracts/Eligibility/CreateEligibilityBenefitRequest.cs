namespace InsureFlow.Contracts.Eligibility;

public sealed class CreateEligibilityBenefitRequest
{
    public string Category { get; set; } = string.Empty;

    public string? ServiceTypeCode { get; set; }

    public string? Description { get; set; }

    public string? CoverageLevel { get; set; }

    public decimal? Copay { get; set; }

    public decimal? CoinsurancePercent { get; set; }

    public decimal? Deductible { get; set; }

    public decimal? Remaining { get; set; }

    public string Status { get; set; } = "Active";
}