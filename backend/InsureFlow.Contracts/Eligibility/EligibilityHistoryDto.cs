namespace InsureFlow.Contracts.Eligibility;

public sealed class EligibilityHistoryDto
{
    public long Id { get; set; }

    public Guid ExternalId { get; set; }

    public long TenantId { get; set; }

    public long EligibilityRequestId { get; set; }

    public string FromStatus { get; set; } = string.Empty;

    public string ToStatus { get; set; } = string.Empty;

    public string? Reason { get; set; }

    public DateTime ChangedAt { get; set; }

    public long? ChangedByUserId { get; set; }
}