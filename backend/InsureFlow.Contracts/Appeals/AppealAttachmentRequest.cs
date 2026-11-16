
namespace InsureFlow.Contracts.Appeals;

public sealed record AppealAttachmentRequest(
    string FileName,
    string ContentType,
    long FileSizeBytes,
    string StorageReference);
