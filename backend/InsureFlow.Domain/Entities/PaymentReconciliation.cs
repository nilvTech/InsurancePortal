
using InsureFlow.Domain.Common;

namespace InsureFlow.Domain.Entities;

public class PaymentReconciliation : AuditableEntity
{
    public long TenantId { get; set; }
    public string ReconciliationNumber { get; set; } = string.Empty;
    public DateTime PeriodStartUtc { get; set; }
    public DateTime PeriodEndUtc { get; set; }
    public DateTime StartedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime? CompletedAtUtc { get; set; }
    public decimal ExpectedAmount { get; set; }
    public decimal ActualAmount { get; set; }
    public decimal DifferenceAmount { get; set; }
    public int PaymentsProcessed { get; set; }
    public int PaymentsMatched { get; set; }
    public int PaymentsUnmatched { get; set; }
    public string Status { get; set; } = "PENDING";
    public string? Summary { get; set; }
    public string? ErrorMessage { get; set; }
}
