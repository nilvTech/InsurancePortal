namespace InsureFlow.Contracts.Eligibility;

/// <summary>
/// Updates editable metadata on an eligibility request.
/// Patient, insurance, payer, and request number remain immutable
/// after the request is created.
/// </summary>
public sealed class UpdateEligibilityRequest
{
    public string? TraceId { get; set; }

    public string? RawRequest { get; set; }
}