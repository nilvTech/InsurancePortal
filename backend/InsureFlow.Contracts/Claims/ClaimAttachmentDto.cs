namespace InsureFlow.Contracts.Claims;

public sealed record ClaimAttachmentInput(
    string FileName,
    string StorageKey,
    string? ContentType,
    long? FileSize,
    string? DocumentType);

public sealed record ClaimAttachmentDto(
    long Id,
    string FileName,
    string? ContentType,
    long? FileSize,
    string DocumentType,
    DateTime UploadedAt);