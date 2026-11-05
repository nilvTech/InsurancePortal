namespace InsureFlow.Contracts.Eligibility;

public sealed class CreateEligibilityRequest
{
    public long PatientId { get; set; }

    public long PatientInsuranceId { get; set; }

    public long PayerId { get; set; }

    public string? RequestNumber { get; set; }

    public string RequestType { get; set; } = "Manual";

    public string Source { get; set; } = "API";

    public string? TraceId { get; set; }

    public string? RawRequest { get; set; }
}