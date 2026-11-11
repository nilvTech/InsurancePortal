
using InsureFlow.Domain.Common;

namespace InsureFlow.Domain.Entities;

public sealed class DenialPayerResponse : AuditableEntity
{
    public long TenantId { get; set; }

    public long DenialId { get; set; }

    public string ResponseType { get; set; } = string.Empty;

    public string? ResponseCode { get; set; }

    public string ResponseMessage { get; set; } = string.Empty;

    public string? PayerReferenceNumber { get; set; }

    public DateTime ResponseDateUtc { get; set; }

    public long? ReceivedByUserId { get; set; }
}
