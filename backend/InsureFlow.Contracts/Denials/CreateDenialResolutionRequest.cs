
namespace InsureFlow.Contracts.Denials;

public sealed class CreateDenialResolutionRequest
{
    public string ResolutionType { get; set; } = string.Empty;

    public string ResolutionNotes { get; set; } = string.Empty;

    public decimal ResolvedAmount { get; set; }

    public DateTime ResolutionDateUtc { get; set; }
}
