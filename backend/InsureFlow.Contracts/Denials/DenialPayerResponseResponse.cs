namespace InsureFlow.Contracts.Denials;

public sealed class DenialPayerResponseResponse
{
    public Guid ExternalId { get; set; }

    public string ResponseType { get; set; } = string.Empty;

    public string? ResponseCode { get; set; }

    public string ResponseMessage { get; set; } = string.Empty;

    public string? PayerReferenceNumber { get; set; }

    public DateTime ResponseDateUtc { get; set; }

    public long? ReceivedByUserId { get; set; }

    public DateTime CreatedAtUtc { get; set; }
}