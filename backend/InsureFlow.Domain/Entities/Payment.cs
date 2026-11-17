
using InsureFlow.Domain.Common;

namespace InsureFlow.Domain.Entities;

public class Payment : AuditableEntity
{
    public long TenantId { get; set; }
    public string PaymentNumber { get; set; } = string.Empty;
    public long PayerId { get; set; }
    public long? PatientId { get; set; }
    public long? PatientInsuranceId { get; set; }
    public string? CheckNumber { get; set; }
    public string? TraceNumber { get; set; }
    public string PaymentMethod { get; set; } = "EFT";
    public decimal PaymentAmount { get; set; }
    public decimal AllocatedAmount { get; set; }
    public decimal UnallocatedAmount { get; set; }
    public string Currency { get; set; } = "USD";
    public DateTime PaymentDateUtc { get; set; }
    public DateTime? ReceivedDateUtc { get; set; }
    public DateTime? PostedAtUtc { get; set; }
    public DateTime? ReversedAtUtc { get; set; }
    public string Status { get; set; } = "DRAFT";
    public string? Notes { get; set; }

    public ICollection<PaymentAllocation> Allocations { get; set; }
        = new List<PaymentAllocation>();

    public void Post()
    {
        if (Status != "DRAFT")
            throw new InvalidOperationException("Only draft payments can be posted.");

        if (PaymentAmount <= 0)
            throw new InvalidOperationException("Payment amount must be greater than zero.");

        Status = "POSTED";
        PostedAtUtc = DateTime.UtcNow;
    }

    public void Reverse()
    {
        if (Status != "POSTED")
            throw new InvalidOperationException("Only posted payments can be reversed.");

        Status = "REVERSED";
        ReversedAtUtc = DateTime.UtcNow;
    }
}
