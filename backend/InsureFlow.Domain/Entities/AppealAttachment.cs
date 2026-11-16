
using InsureFlow.Domain.Common;

namespace InsureFlow.Domain.Entities;

public sealed class AppealAttachment : AuditableEntity
{
    private AppealAttachment()
    {
    }

    public AppealAttachment(
        long tenantId,
        long appealId,
        string fileName,
        string contentType,
        long fileSizeBytes,
        string storageReference,
        long? uploadedByUserId = null)
    {
        if (tenantId <= 0)
            throw new ArgumentOutOfRangeException(nameof(tenantId));

        if (appealId <= 0)
            throw new ArgumentOutOfRangeException(nameof(appealId));

        if (string.IsNullOrWhiteSpace(fileName))
            throw new ArgumentException(
                "File name is required.",
                nameof(fileName));

        if (string.IsNullOrWhiteSpace(contentType))
            throw new ArgumentException(
                "Content type is required.",
                nameof(contentType));

        if (fileSizeBytes <= 0)
            throw new ArgumentOutOfRangeException(
                nameof(fileSizeBytes),
                "File size must be greater than zero.");

        if (string.IsNullOrWhiteSpace(storageReference))
            throw new ArgumentException(
                "Storage reference is required.",
                nameof(storageReference));

        TenantId = tenantId;
        AppealId = appealId;
        FileName = fileName.Trim();
        ContentType = contentType.Trim();
        FileSizeBytes = fileSizeBytes;
        StorageReference = storageReference.Trim();
        UploadedAtUtc = DateTime.UtcNow;
        UploadedByUserId = uploadedByUserId;
    }

    public long TenantId { get; private set; }

    public long AppealId { get; private set; }

    public string FileName { get; private set; } = string.Empty;

    public string ContentType { get; private set; } = string.Empty;

    public long FileSizeBytes { get; private set; }

    public string StorageReference { get; private set; } = string.Empty;

    public DateTime UploadedAtUtc { get; private set; }

    public long? UploadedByUserId { get; private set; }
}
