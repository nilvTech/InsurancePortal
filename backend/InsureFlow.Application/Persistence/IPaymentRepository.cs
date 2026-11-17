
using InsureFlow.Domain.Entities;

namespace InsureFlow.Application.Persistence;

public interface IPaymentRepository
{
    // Payments
    Task<List<Payment>> GetPaymentsAsync(CancellationToken cancellationToken = default);
    Task<Payment?> GetPaymentAsync(Guid externalId, CancellationToken cancellationToken = default);
    Task AddPaymentAsync(Payment payment, CancellationToken cancellationToken = default);

    // Allocations
    Task<List<PaymentAllocation>> GetAllocationsAsync(long paymentId, CancellationToken cancellationToken = default);
    Task<PaymentAllocation?> GetAllocationAsync(long paymentId, Guid externalId, CancellationToken cancellationToken = default);
    Task AddAllocationAsync(PaymentAllocation allocation, CancellationToken cancellationToken = default);

    // ERA
    Task<List<EraRecord>> GetEraRecordsAsync(CancellationToken cancellationToken = default);
    Task<EraRecord?> GetEraRecordAsync(Guid externalId, CancellationToken cancellationToken = default);
    Task AddEraRecordAsync(EraRecord era, CancellationToken cancellationToken = default);

    // EOB
    Task<List<EobRecord>> GetEobRecordsAsync(CancellationToken cancellationToken = default);
    Task<EobRecord?> GetEobRecordAsync(Guid externalId, CancellationToken cancellationToken = default);
    Task AddEobRecordAsync(EobRecord eob, CancellationToken cancellationToken = default);

    // Adjustments
    Task<List<PaymentAdjustment>> GetAdjustmentsAsync(CancellationToken cancellationToken = default);
    Task<PaymentAdjustment?> GetAdjustmentAsync(Guid externalId, CancellationToken cancellationToken = default);
    Task AddAdjustmentAsync(PaymentAdjustment adjustment, CancellationToken cancellationToken = default);

    // Reconciliation
    Task<List<PaymentReconciliation>> GetReconciliationsAsync(CancellationToken cancellationToken = default);
    Task<PaymentReconciliation?> GetReconciliationAsync(Guid externalId, CancellationToken cancellationToken = default);
    Task AddReconciliationAsync(PaymentReconciliation reconciliation, CancellationToken cancellationToken = default);

    // Refunds
    Task<List<Refund>> GetRefundsAsync(CancellationToken cancellationToken = default);
    Task<Refund?> GetRefundAsync(Guid externalId, CancellationToken cancellationToken = default);
    Task AddRefundAsync(Refund refund, CancellationToken cancellationToken = default);

    // Existing entity lookups
    Task<Payer?> GetPayerAsync(Guid externalId, CancellationToken cancellationToken = default);
    Task<Patient?> GetPatientAsync(Guid externalId, CancellationToken cancellationToken = default);
    Task<PatientInsurance?> GetPatientInsuranceAsync(Guid externalId, CancellationToken cancellationToken = default);
    Task<Claim?> GetClaimAsync(Guid externalId, CancellationToken cancellationToken = default);

    // Persistence
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    Task<Payer?> GetPayerByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<Patient?> GetPatientByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<PatientInsurance?> GetPatientInsuranceByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<Claim?> GetClaimByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<ClaimLine?> GetClaimLineByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<Payment?> GetPaymentByIdAsync(long id, CancellationToken cancellationToken = default);

    Task<ClaimLine?> GetClaimLineAsync(Guid externalId, CancellationToken cancellationToken = default);

    

}
