
namespace InsureFlow.Contracts.Payments;

public sealed record PaymentAdjustmentQueryRequest(
    Guid? PaymentId = null,
    Guid? ClaimId = null,
    string? AdjustmentCode = null,
    string? Status = null,
    int Page = 1,
    int PageSize = 20);

public sealed record CreatePaymentAdjustmentRequest(
    Guid PaymentId,
    Guid? PaymentAllocationId,
    Guid? ClaimId,
    string AdjustmentCode,
    string? AdjustmentGroupCode,
    string Description,
    decimal AdjustmentAmount,
    DateTime AdjustmentDateUtc,
    string? Notes);

public sealed record UpdatePaymentAdjustmentRequest(
    string AdjustmentCode,
    string? AdjustmentGroupCode,
    string Description,
    decimal AdjustmentAmount,
    DateTime AdjustmentDateUtc,
    string? Status,
    string? Notes);

public sealed record PaymentAdjustmentResponse(
    Guid Id,
    Guid PaymentId,
    Guid? PaymentAllocationId,
    Guid? ClaimId,
    string AdjustmentCode,
    string? AdjustmentGroupCode,
    string Description,
    decimal AdjustmentAmount,
    DateTime AdjustmentDateUtc,
    string Status,
    string? Notes);
