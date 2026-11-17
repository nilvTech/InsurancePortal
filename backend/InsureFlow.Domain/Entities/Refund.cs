
using InsureFlow.Domain.Common;

namespace InsureFlow.Domain.Entities;

public class Refund : AuditableEntity
{
    public long TenantId { get; set; }
    public long PaymentId { get; set; }
    public string RefundNumber { get; set; } = string.Empty;
    public decimal RefundAmount { get; set; }
    public string Reason { get; set; } = string.Empty;
    public string Status { get; set; } = "PENDING";
    public DateTime RequestedAtUtc { get; set; } = DateTime.UtcNow;
    public long? RequestedByUserId { get; set; }
    public DateTime? ReviewedAtUtc { get; set; }
    public long? ReviewedByUserId { get; set; }
    public string? ReviewNotes { get; set; }
    public DateTime? ProcessedAtUtc { get; set; }

    public void Approve(long? reviewerUserId, string? notes = null)
    {
        if (Status != "PENDING")
            throw new InvalidOperationException("Only pending refunds can be approved.");

        Status = "APPROVED";
        ReviewedAtUtc = DateTime.UtcNow;
        ReviewedByUserId = reviewerUserId;
        ReviewNotes = notes;
    }

    public void Reject(long? reviewerUserId, string? notes = null)
    {
        if (Status != "PENDING")
            throw new InvalidOperationException("Only pending refunds can be rejected.");

        Status = "REJECTED";
        ReviewedAtUtc = DateTime.UtcNow;
        ReviewedByUserId = reviewerUserId;
        ReviewNotes = notes;
    }
}
