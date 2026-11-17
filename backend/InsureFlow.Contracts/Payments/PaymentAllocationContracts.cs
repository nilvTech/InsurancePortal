
namespace InsureFlow.Contracts.Payments;

public sealed record CreatePaymentAllocationRequest(
    Guid ClaimId,
    Guid? ClaimLineId,
    decimal AllocatedAmount,
    string? Notes);

public sealed record UpdatePaymentAllocationRequest(
    decimal AllocatedAmount,
    string? Status,
    string? Notes);

public sealed record PaymentAllocationResponse(
    Guid Id,
    Guid PaymentId,
    Guid ClaimId,
    Guid? ClaimLineId,
    decimal AllocatedAmount,
    DateTime AllocationDateUtc,
    string Status,
    string? Notes,
    DateTime CreatedAtUtc);
