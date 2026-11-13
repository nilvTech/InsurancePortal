
namespace InsureFlow.Contracts.Denials;

public sealed class DenialResolutionResponse
{
    public Guid ExternalId { get; set; }

    public string ResolutionType { get; set; } = string.Empty;

    public string ResolutionNotes { get; set; } = string.Empty;

    public decimal ResolvedAmount { get; set; }

    public DateTime ResolutionDateUtc { get; set; }

    public long? ResolvedByUserId { get; set; }

    public DateTime CreatedAtUtc { get; set; }
}
