namespace InsureFlow.Contracts.Eligibility;

public sealed class EligibilityBenefitDto
{
    public long Id { get; set; }

    public Guid ExternalId { get; set; }

    public long TenantId { get; set; }

    public long EligibilityResponseId { get; set; }

    public string Category { get; set; } = string.Empty;

    public string? ServiceTypeCode { get; set; }

    public string? Description { get; set; }

    public string? CoverageLevel { get; set; }

    public decimal? Copay { get; set; }

    public decimal? CoinsurancePercent { get; set; }

    public decimal? Deductible { get; set; }

    public decimal? Remaining { get; set; }

    public string Status { get; set; } = string.Empty;
}