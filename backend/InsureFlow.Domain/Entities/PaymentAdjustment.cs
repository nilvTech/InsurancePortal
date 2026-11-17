
using InsureFlow.Domain.Common;

namespace InsureFlow.Domain.Entities;

public class PaymentAdjustment : AuditableEntity
{
    public long TenantId { get; set; }
    public long PaymentId { get; set; }
    public long? PaymentAllocationId { get; set; }
    public long? ClaimId { get; set; }
    public string AdjustmentCode { get; set; } = string.Empty;
    public string? AdjustmentGroupCode { get; set; }
    public string Description { get; set; } = string.Empty;
    public decimal AdjustmentAmount { get; set; }
    public DateTime AdjustmentDateUtc { get; set; } = DateTime.UtcNow;
    public string Status { get; set; } = "ACTIVE";
    public string? Notes { get; set; }
}
