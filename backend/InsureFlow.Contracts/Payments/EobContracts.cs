
namespace InsureFlow.Contracts.Payments;

public sealed record EobQueryRequest(
    string? Search = null,
    string? Status = null,
    Guid? PayerId = null,
    Guid? ClaimId = null,
    DateTime? FromDateUtc = null,
    DateTime? ToDateUtc = null,
    int Page = 1,
    int PageSize = 20);

public sealed record UploadEobRequest(
    Guid PayerId,
    Guid? PatientId,
    Guid? ClaimId,
    string? DocumentNumber,
    string FileName,
    string ContentType,
    string StorageReference,
    long? FileSizeBytes,
    DateTime? StatementDateUtc,
    string? Notes);

public sealed record EobResponse(
    Guid Id,
    Guid PayerId,
    Guid? PatientId,
    Guid? ClaimId,
    string? DocumentNumber,
    string? FileName,
    string? ContentType,
    string? StorageReference,
    long? FileSizeBytes,
    DateTime ReceivedAtUtc,
    DateTime? StatementDateUtc,
    string Status,
    string? Notes);

public sealed record EobListResponse(
    IReadOnlyList<EobResponse> Items,
    int Page,
    int PageSize,
    int TotalCount);
