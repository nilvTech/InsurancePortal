
namespace InsureFlow.Contracts.Payments;

public sealed record PaymentQueryRequest(
    string? Search = null,
    string? Status = null,
    Guid? PayerId = null,
    DateTime? FromDateUtc = null,
    DateTime? ToDateUtc = null,
    int Page = 1,
    int PageSize = 20);

public sealed record CreatePaymentRequest(
    Guid PayerId,
    Guid? PatientId,
    Guid? PatientInsuranceId,
    string PaymentNumber,
    string? CheckNumber,
    string? TraceNumber,
    string PaymentMethod,
    decimal PaymentAmount,
    string Currency,
    DateTime PaymentDateUtc,
    DateTime? ReceivedDateUtc,
    string? Notes);

public sealed record UpdatePaymentRequest(
    string? CheckNumber,
    string? TraceNumber,
    string PaymentMethod,
    decimal PaymentAmount,
    string Currency,
    DateTime PaymentDateUtc,
    DateTime? ReceivedDateUtc,
    string? Notes);

public sealed record PaymentResponse(
    Guid Id,
    string PaymentNumber,
    Guid PayerId,
    Guid? PatientId,
    Guid? PatientInsuranceId,
    string? CheckNumber,
    string? TraceNumber,
    string PaymentMethod,
    decimal PaymentAmount,
    decimal AllocatedAmount,
    decimal UnallocatedAmount,
    string Currency,
    DateTime PaymentDateUtc,
    DateTime? ReceivedDateUtc,
    DateTime? PostedAtUtc,
    DateTime? ReversedAtUtc,
    string Status,
    string? Notes,
    DateTime CreatedAtUtc);

public sealed record PaymentListResponse(
    IReadOnlyList<PaymentResponse> Items,
    int Page,
    int PageSize,
    int TotalCount);
