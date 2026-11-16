
namespace InsureFlow.Contracts.Appeals;

public sealed record AppealAttachmentResponse(
    Guid ExternalId,
    Guid AppealExternalId,
    string FileName,
    string ContentType,
    long FileSizeBytes,
    string StorageReference,
    DateTime UploadedAtUtc,
    long? UploadedByUserId);
