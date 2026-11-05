using InsureFlow.Domain.Common;
using InsureFlow.Domain.Enums;

namespace InsureFlow.Domain.Entities;

public sealed class EligibilityResponse : AuditableEntity
{
    private EligibilityResponse()
    {
    }

    public EligibilityResponse(
        long tenantId,
        long eligibilityRequestId,
        EligibilityResponseStatus status,
        DateTime respondedAtUtc)
    {
        if (tenantId <= 0)
        {
            throw new ArgumentException(
                "Tenant ID is required.",
                nameof(tenantId));
        }

        if (eligibilityRequestId <= 0)
        {
            throw new ArgumentException(
                "Eligibility request ID is required.",
                nameof(eligibilityRequestId));
        }

        TenantId = tenantId;
        EligibilityRequestId = eligibilityRequestId;
        Status = status;
        RespondedAt = respondedAtUtc;

        CreatedAtUtc = respondedAtUtc;
        UpdatedAtUtc = respondedAtUtc;
    }

    public long TenantId { get; private set; }

    public long EligibilityRequestId { get; private set; }

    public EligibilityResponseStatus Status { get; private set; }

    public string? PayerReference { get; private set; }

    public CoverageStatus? CoverageStatus { get; private set; }

    public DateTime? EffectiveDate { get; private set; }

    public DateTime? TerminationDate { get; private set; }

    public decimal? DeductibleIndividual { get; private set; }

    public decimal? DeductibleRemaining { get; private set; }

    public decimal? Copay { get; private set; }

    public decimal? CoinsurancePercent { get; private set; }

    public string? ResponseMessage { get; private set; }

    public string? RawResponse { get; private set; }

    public DateTime RespondedAt { get; private set; }

    public Tenant Tenant { get; private set; } = null!;

    public EligibilityRequest EligibilityRequest { get; private set; }
        = null!;

    public ICollection<EligibilityBenefit> Benefits { get; private set; }
        = new List<EligibilityBenefit>();

    public void SetCoverage(
        string? payerReference,
        CoverageStatus? coverageStatus,
        DateTime? effectiveDate,
        DateTime? terminationDate)
    {
        if (effectiveDate.HasValue &&
            terminationDate.HasValue &&
            terminationDate.Value < effectiveDate.Value)
        {
            throw new ArgumentException(
                "Termination date cannot be earlier than effective date.",
                nameof(terminationDate));
        }

        PayerReference = NormalizeOptional(payerReference);
        CoverageStatus = coverageStatus;
        EffectiveDate = effectiveDate;
        TerminationDate = terminationDate;

        UpdatedAtUtc = DateTime.UtcNow;
    }

    public void SetFinancialDetails(
        decimal? deductibleIndividual,
        decimal? deductibleRemaining,
        decimal? copay,
        decimal? coinsurancePercent)
    {
        if (deductibleIndividual.HasValue &&
            deductibleIndividual.Value < 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(deductibleIndividual));
        }

        if (deductibleRemaining.HasValue &&
            deductibleRemaining.Value < 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(deductibleRemaining));
        }

        if (copay.HasValue &&
            copay.Value < 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(copay));
        }

        if (coinsurancePercent.HasValue &&
            (coinsurancePercent.Value < 0 ||
             coinsurancePercent.Value > 100))
        {
            throw new ArgumentOutOfRangeException(
                nameof(coinsurancePercent));
        }

        DeductibleIndividual = deductibleIndividual;
        DeductibleRemaining = deductibleRemaining;
        Copay = copay;
        CoinsurancePercent = coinsurancePercent;

        UpdatedAtUtc = DateTime.UtcNow;
    }

    public void SetResponseMessage(string? responseMessage)
    {
        ResponseMessage = NormalizeOptional(responseMessage);
        UpdatedAtUtc = DateTime.UtcNow;
    }

    public void SetRawResponse(string? rawResponse)
    {
        RawResponse = rawResponse;
        UpdatedAtUtc = DateTime.UtcNow;
    }

    public void MarkVerified(DateTime respondedAtUtc)
    {
        Status = EligibilityResponseStatus.Verified;
        RespondedAt = respondedAtUtc;
        UpdatedAtUtc = respondedAtUtc;
    }

    public void MarkInactive(DateTime respondedAtUtc)
    {
        Status = EligibilityResponseStatus.Inactive;
        RespondedAt = respondedAtUtc;
        UpdatedAtUtc = respondedAtUtc;
    }

    public void MarkError(
        string? responseMessage,
        DateTime respondedAtUtc)
    {
        Status = EligibilityResponseStatus.Error;
        ResponseMessage = NormalizeOptional(responseMessage);
        RespondedAt = respondedAtUtc;
        UpdatedAtUtc = respondedAtUtc;
    }

    private static string? NormalizeOptional(string? value)
    {
        return string.IsNullOrWhiteSpace(value)
            ? null
            : value.Trim();
    }
}