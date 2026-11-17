
using InsureFlow.Domain.Common;

namespace InsureFlow.Domain.Entities;

public class PaymentAllocation : AuditableEntity
{
    public long TenantId { get; set; }
    public long PaymentId { get; set; }
    public long ClaimId { get; set; }
    public long? ClaimLineId { get; set; }
    public decimal AllocatedAmount { get; set; }
    public DateTime AllocationDateUtc { get; set; } = DateTime.UtcNow;
    public string Status { get; set; } = "ACTIVE";
    public string? Notes { get; set; }

    public Payment? Payment { get; set; }
}
