
using InsureFlow.Application.Persistence;
using InsureFlow.Domain.Entities;
using InsureFlow.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace InsureFlow.Infrastructure.Persistence.Repositories;

public sealed class PaymentRepository : IPaymentRepository
{
    private readonly InsureFlowDbContext _context;

    public PaymentRepository(InsureFlowDbContext context)
    {
        _context = context;
    }

    // Payments
    public Task<List<Payment>> GetPaymentsAsync(CancellationToken cancellationToken = default)
        => _context.Payments.AsNoTracking().ToListAsync(cancellationToken);

    public Task<Payment?> GetPaymentAsync(Guid externalId, CancellationToken cancellationToken = default)
        => _context.Payments.FirstOrDefaultAsync(x => x.ExternalId == externalId, cancellationToken);

    public async Task AddPaymentAsync(Payment payment, CancellationToken cancellationToken = default)
        => await _context.Payments.AddAsync(payment, cancellationToken);

    // Allocations
    public Task<List<PaymentAllocation>> GetAllocationsAsync(long paymentId, CancellationToken cancellationToken = default)
        => _context.PaymentAllocations
            .AsNoTracking()
            .Where(x => x.PaymentId == paymentId)
            .ToListAsync(cancellationToken);

    public Task<PaymentAllocation?> GetAllocationAsync(
        long paymentId,
        Guid externalId,
        CancellationToken cancellationToken = default)
        => _context.PaymentAllocations.FirstOrDefaultAsync(
            x => x.PaymentId == paymentId && x.ExternalId == externalId,
            cancellationToken);

    public async Task AddAllocationAsync(
        PaymentAllocation allocation,
        CancellationToken cancellationToken = default)
        => await _context.PaymentAllocations.AddAsync(allocation, cancellationToken);

    // ERA
    public Task<List<EraRecord>> GetEraRecordsAsync(CancellationToken cancellationToken = default)
        => _context.EraRecords.AsNoTracking().ToListAsync(cancellationToken);

    public Task<EraRecord?> GetEraRecordAsync(Guid externalId, CancellationToken cancellationToken = default)
        => _context.EraRecords.FirstOrDefaultAsync(x => x.ExternalId == externalId, cancellationToken);

    public async Task AddEraRecordAsync(EraRecord era, CancellationToken cancellationToken = default)
        => await _context.EraRecords.AddAsync(era, cancellationToken);

    // EOB
    public Task<List<EobRecord>> GetEobRecordsAsync(CancellationToken cancellationToken = default)
        => _context.EobRecords.AsNoTracking().ToListAsync(cancellationToken);

    public Task<EobRecord?> GetEobRecordAsync(Guid externalId, CancellationToken cancellationToken = default)
        => _context.EobRecords.FirstOrDefaultAsync(x => x.ExternalId == externalId, cancellationToken);

    public async Task AddEobRecordAsync(EobRecord eob, CancellationToken cancellationToken = default)
        => await _context.EobRecords.AddAsync(eob, cancellationToken);

    // Adjustments
    public Task<List<PaymentAdjustment>> GetAdjustmentsAsync(CancellationToken cancellationToken = default)
        => _context.PaymentAdjustments.AsNoTracking().ToListAsync(cancellationToken);

    public Task<PaymentAdjustment?> GetAdjustmentAsync(Guid externalId, CancellationToken cancellationToken = default)
        => _context.PaymentAdjustments.FirstOrDefaultAsync(x => x.ExternalId == externalId, cancellationToken);

    public async Task AddAdjustmentAsync(
        PaymentAdjustment adjustment,
        CancellationToken cancellationToken = default)
        => await _context.PaymentAdjustments.AddAsync(adjustment, cancellationToken);

    // Reconciliation
    public Task<List<PaymentReconciliation>> GetReconciliationsAsync(CancellationToken cancellationToken = default)
        => _context.PaymentReconciliations.AsNoTracking().ToListAsync(cancellationToken);

    public Task<PaymentReconciliation?> GetReconciliationAsync(
        Guid externalId,
        CancellationToken cancellationToken = default)
        => _context.PaymentReconciliations.FirstOrDefaultAsync(
            x => x.ExternalId == externalId,
            cancellationToken);

    public async Task AddReconciliationAsync(
        PaymentReconciliation reconciliation,
        CancellationToken cancellationToken = default)
        => await _context.PaymentReconciliations.AddAsync(reconciliation, cancellationToken);

    // Refunds
    public Task<List<Refund>> GetRefundsAsync(CancellationToken cancellationToken = default)
        => _context.Refunds.AsNoTracking().ToListAsync(cancellationToken);

    public Task<Refund?> GetRefundAsync(Guid externalId, CancellationToken cancellationToken = default)
        => _context.Refunds.FirstOrDefaultAsync(x => x.ExternalId == externalId, cancellationToken);

    public async Task AddRefundAsync(Refund refund, CancellationToken cancellationToken = default)
        => await _context.Refunds.AddAsync(refund, cancellationToken);

    // Existing entity lookups
    public Task<Payer?> GetPayerAsync(Guid externalId, CancellationToken cancellationToken = default)
        => _context.Payers.FirstOrDefaultAsync(x => x.ExternalId == externalId, cancellationToken);

    public Task<Patient?> GetPatientAsync(Guid externalId, CancellationToken cancellationToken = default)
        => _context.Patients.FirstOrDefaultAsync(x => x.ExternalId == externalId, cancellationToken);

    public Task<PatientInsurance?> GetPatientInsuranceAsync(
        Guid externalId,
        CancellationToken cancellationToken = default)
        => _context.PatientInsurances.FirstOrDefaultAsync(
            x => x.ExternalId == externalId,
            cancellationToken);

    public Task<Claim?> GetClaimAsync(Guid externalId, CancellationToken cancellationToken = default)
        => _context.Claims.FirstOrDefaultAsync(x => x.ExternalId == externalId, cancellationToken);

    // Persistence
    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        => _context.SaveChangesAsync(cancellationToken);

    public Task<ClaimLine?> GetClaimLineAsync(
Guid externalId,
CancellationToken cancellationToken = default)
=> _context.ClaimLines.FirstOrDefaultAsync(
    x => x.ExternalId == externalId,
    cancellationToken);

    public Task<Payer?> GetPayerByIdAsync(long id, CancellationToken cancellationToken = default)
    => _context.Payers.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public Task<Patient?> GetPatientByIdAsync(long id, CancellationToken cancellationToken = default)
        => _context.Patients.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public Task<PatientInsurance?> GetPatientInsuranceByIdAsync(long id, CancellationToken cancellationToken = default)
        => _context.PatientInsurances.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public Task<Claim?> GetClaimByIdAsync(long id, CancellationToken cancellationToken = default)
        => _context.Claims.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public Task<ClaimLine?> GetClaimLineByIdAsync(long id, CancellationToken cancellationToken = default)
        => _context.ClaimLines.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public Task<Payment?> GetPaymentByIdAsync(long id, CancellationToken cancellationToken = default)
        => _context.Payments.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        
}
