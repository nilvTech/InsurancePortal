
using InsureFlow.Domain.Common;

namespace InsureFlow.Domain.Entities;

public class EraRecord : AuditableEntity
{
    public long TenantId { get; set; }
    public long PayerId { get; set; }
    public string? TraceNumber { get; set; }
    public string? CheckNumber { get; set; }
    public decimal TotalPaymentAmount { get; set; }
    public DateTime? PaymentDateUtc { get; set; }
    public DateTime ReceivedAtUtc { get; set; } = DateTime.UtcNow;
    public string? FileName { get; set; }
    public string? StorageReference { get; set; }
    public string? TransactionControlNumber { get; set; }
    public string ImportStatus { get; set; } = "RECEIVED";
    public string? ErrorMessage { get; set; }
    public string? RawContent { get; set; }
}
