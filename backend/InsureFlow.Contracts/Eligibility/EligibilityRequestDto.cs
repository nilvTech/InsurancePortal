namespace InsureFlow.Contracts.Eligibility;

public sealed class EligibilityRequestDto
{
    public long Id { get; set; }

    public Guid ExternalId { get; set; }

    public long TenantId { get; set; }

    public long PatientId { get; set; }

    public long PatientInsuranceId { get; set; }

    public long PayerId { get; set; }

    public string RequestNumber { get; set; } = string.Empty;

    public string RequestType { get; set; } = string.Empty;

    public string Status { get; set; } = string.Empty;

    public string Source { get; set; } = string.Empty;

    public DateTime RequestedAt { get; set; }

    public DateTime? CompletedAt { get; set; }

    public string? ErrorCode { get; set; }

    public string? ErrorMessage { get; set; }

    public string? TraceId { get; set; }

    public EligibilityResponseDto? Response { get; set; }

    public IReadOnlyList<EligibilityHistoryDto> History { get; set; }
        = Array.Empty<EligibilityHistoryDto>();
}