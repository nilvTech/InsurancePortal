
namespace InsureFlow.Contracts.Payments;

public sealed record RunPaymentReconciliationRequest(
    DateTime PeriodStartUtc,
    DateTime PeriodEndUtc,
    decimal? ExpectedAmount = null);

public sealed record PaymentReconciliationResponse(
    Guid Id,
    string ReconciliationNumber,
    DateTime PeriodStartUtc,
    DateTime PeriodEndUtc,
    DateTime StartedAtUtc,
    DateTime? CompletedAtUtc,
    decimal ExpectedAmount,
    decimal ActualAmount,
    decimal DifferenceAmount,
    int PaymentsProcessed,
    int PaymentsMatched,
    int PaymentsUnmatched,
    string Status,
    string? Summary,
    string? ErrorMessage);

public sealed record PaymentReconciliationListResponse(
    IReadOnlyList<PaymentReconciliationResponse> Items,
    int Page,
    int PageSize,
    int TotalCount);
