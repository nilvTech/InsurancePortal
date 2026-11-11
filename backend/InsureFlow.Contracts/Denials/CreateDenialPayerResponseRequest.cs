namespace InsureFlow.Contracts.Denials;

public sealed class CreateDenialPayerResponseRequest
{
    public string ResponseType { get; set; } = string.Empty;

    public string? ResponseCode { get; set; }

    public string ResponseMessage { get; set; } = string.Empty;

    public string? PayerReferenceNumber { get; set; }

    public DateTime ResponseDateUtc { get; set; }
}