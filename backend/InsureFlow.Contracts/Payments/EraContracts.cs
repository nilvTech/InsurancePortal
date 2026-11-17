
namespace InsureFlow.Contracts.Payments;

public sealed record EraQueryRequest(
    string? Search = null,
    string? ImportStatus = null,
    Guid? PayerId = null,
    DateTime? FromDateUtc = null,
    DateTime? ToDateUtc = null,
    int Page = 1,
    int PageSize = 20);

public sealed record ImportEraRequest(
    Guid PayerId,
    string FileName,
    string RawContent);

public sealed record EraResponse(
    Guid Id,
    Guid PayerId,
    string? TraceNumber,
    string? CheckNumber,
    decimal TotalPaymentAmount,
    DateTime? PaymentDateUtc,
    DateTime ReceivedAtUtc,
    string? FileName,
    string? StorageReference,
    string? TransactionControlNumber,
    string ImportStatus,
    string? ErrorMessage);

public sealed record EraListResponse(
    IReadOnlyList<EraResponse> Items,
    int Page,
    int PageSize,
    int TotalCount);
