namespace InsureFlow.Contracts.Eligibility;

public sealed class RecordEligibilityResponseRequest
{
    /// <summary>
    /// Verified, NotVerified, Inactive, or Error.
    /// </summary>
    public string Status { get; set; } = "Verified";

    public string? PayerReference { get; set; }

    /// <summary>
    /// Active, Inactive, Terminated, or Unknown.
    /// </summary>
    public string? CoverageStatus { get; set; }

    public DateTime? EffectiveDate { get; set; }

    public DateTime? TerminationDate { get; set; }

    public decimal? DeductibleIndividual { get; set; }

    public decimal? DeductibleRemaining { get; set; }

    public decimal? Copay { get; set; }

    public decimal? CoinsurancePercent { get; set; }

    public string? ResponseMessage { get; set; }

    public string? RawResponse { get; set; }

    public IReadOnlyList<CreateEligibilityBenefitRequest> Benefits { get; set; }
        = Array.Empty<CreateEligibilityBenefitRequest>();
}