
using InsureFlow.Domain.Common;

namespace InsureFlow.Domain.Entities;

public class EobRecord : AuditableEntity
{
    public long TenantId { get; set; }
    public long PayerId { get; set; }
    public long? PatientId { get; set; }
    public long? ClaimId { get; set; }
    public string? DocumentNumber { get; set; }
    public string? FileName { get; set; }
    public string? ContentType { get; set; }
    public string? StorageReference { get; set; }
    public long? FileSizeBytes { get; set; }
    public DateTime ReceivedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime? StatementDateUtc { get; set; }
    public string Status { get; set; } = "RECEIVED";
    public string? Notes { get; set; }
}
