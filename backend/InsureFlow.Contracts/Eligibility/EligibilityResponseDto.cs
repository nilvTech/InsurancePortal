namespace InsureFlow.Contracts.Eligibility;

public sealed class EligibilityResponseDto
{
    public long Id { get; set; }

    public Guid ExternalId { get; set; }

    public long TenantId { get; set; }

    public long EligibilityRequestId { get; set; }

    public string Status { get; set; } = string.Empty;

    public string? PayerReference { get; set; }

    public string? CoverageStatus { get; set; }

    public DateTime? EffectiveDate { get; set; }

    public DateTime? TerminationDate { get; set; }

    public decimal? DeductibleIndividual { get; set; }

    public decimal? DeductibleRemaining { get; set; }

    public decimal? Copay { get; set; }

    public decimal? CoinsurancePercent { get; set; }

    public string? ResponseMessage { get; set; }

    public DateTime RespondedAt { get; set; }

    public IReadOnlyList<EligibilityBenefitDto> Benefits { get; set; }
        = Array.Empty<EligibilityBenefitDto>();
}