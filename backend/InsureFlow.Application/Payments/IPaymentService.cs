
using InsureFlow.Contracts.Payments;

namespace InsureFlow.Application.Payments;

public interface IPaymentService
{
    // Payments
    Task<PaymentListResponse> GetPaymentsAsync(PaymentQueryRequest request, CancellationToken cancellationToken = default);
    Task<PaymentResponse?> GetPaymentAsync(Guid id, CancellationToken cancellationToken = default);
    Task<PaymentResponse> CreatePaymentAsync(CreatePaymentRequest request, CancellationToken cancellationToken = default);
    Task<PaymentResponse?> UpdatePaymentAsync(Guid id, UpdatePaymentRequest request, CancellationToken cancellationToken = default);
    Task<PaymentResponse?> PostPaymentAsync(Guid id, CancellationToken cancellationToken = default);
    Task<PaymentResponse?> ReversePaymentAsync(Guid id, CancellationToken cancellationToken = default);

    // Allocations
    Task<IReadOnlyList<PaymentAllocationResponse>?> GetAllocationsAsync(Guid paymentId, CancellationToken cancellationToken = default);
    Task<PaymentAllocationResponse?> CreateAllocationAsync(Guid paymentId, CreatePaymentAllocationRequest request, CancellationToken cancellationToken = default);
    Task<PaymentAllocationResponse?> UpdateAllocationAsync(Guid paymentId, Guid allocationId, UpdatePaymentAllocationRequest request, CancellationToken cancellationToken = default);

    // ERA
    Task<EraListResponse> GetEraRecordsAsync(EraQueryRequest request, CancellationToken cancellationToken = default);
    Task<EraResponse?> GetEraRecordAsync(Guid id, CancellationToken cancellationToken = default);
    Task<EraResponse> ImportEraAsync(ImportEraRequest request, CancellationToken cancellationToken = default);

    // EOB
    Task<EobListResponse> GetEobRecordsAsync(EobQueryRequest request, CancellationToken cancellationToken = default);
    Task<EobResponse?> GetEobRecordAsync(Guid id, CancellationToken cancellationToken = default);
    Task<EobResponse> UploadEobAsync(UploadEobRequest request, CancellationToken cancellationToken = default);

    // Adjustments
    Task<IReadOnlyList<PaymentAdjustmentResponse>> GetAdjustmentsAsync(PaymentAdjustmentQueryRequest request, CancellationToken cancellationToken = default);
    Task<PaymentAdjustmentResponse?> GetAdjustmentAsync(Guid id, CancellationToken cancellationToken = default);
    Task<PaymentAdjustmentResponse> CreateAdjustmentAsync(CreatePaymentAdjustmentRequest request, CancellationToken cancellationToken = default);
    Task<PaymentAdjustmentResponse?> UpdateAdjustmentAsync(Guid id, UpdatePaymentAdjustmentRequest request, CancellationToken cancellationToken = default);

    // Reconciliation
    Task<PaymentReconciliationListResponse> GetReconciliationsAsync(int page = 1, int pageSize = 20, CancellationToken cancellationToken = default);
    Task<PaymentReconciliationResponse> RunReconciliationAsync(RunPaymentReconciliationRequest request, CancellationToken cancellationToken = default);

    // Refunds
    Task<IReadOnlyList<RefundResponse>> GetRefundsAsync(RefundQueryRequest request, CancellationToken cancellationToken = default);
    Task<RefundResponse?> GetRefundAsync(Guid id, CancellationToken cancellationToken = default);
    Task<RefundResponse> CreateRefundAsync(CreateRefundRequest request, CancellationToken cancellationToken = default);
    Task<RefundResponse?> ApproveRefundAsync(Guid id, ReviewRefundRequest request, CancellationToken cancellationToken = default);
    Task<RefundResponse?> RejectRefundAsync(Guid id, ReviewRefundRequest request, CancellationToken cancellationToken = default);
}
