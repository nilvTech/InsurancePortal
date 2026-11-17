
namespace InsureFlow.Contracts.Payments;

public sealed record RefundQueryRequest(
    Guid? PaymentId = null,
    string? Status = null,
    DateTime? FromDateUtc = null,
    DateTime? ToDateUtc = null,
    int Page = 1,
    int PageSize = 20);

public sealed record CreateRefundRequest(
    Guid PaymentId,
    decimal RefundAmount,
    string Reason);

public sealed record ReviewRefundRequest(
    string? ReviewNotes);

public sealed record RefundResponse(
    Guid Id,
    Guid PaymentId,
    string RefundNumber,
    decimal RefundAmount,
    string Reason,
    string Status,
    DateTime RequestedAtUtc,
    long? RequestedByUserId,
    DateTime? ReviewedAtUtc,
    long? ReviewedByUserId,
    string? ReviewNotes,
    DateTime? ProcessedAtUtc);
