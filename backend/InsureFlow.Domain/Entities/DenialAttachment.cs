
using InsureFlow.Domain.Common;

namespace InsureFlow.Domain.Entities;

public sealed class DenialAttachment : AuditableEntity
{
    private DenialAttachment()
    {
    }

    public DenialAttachment(
        long tenantId,
        long denialId,
        string fileName,
        string contentType,
        long fileSizeBytes,
        string storageReference,
        long? uploadedByUserId = null)
    {
        if (tenantId <= 0)
            throw new ArgumentException("Tenant ID must be greater than zero.");

        if (denialId <= 0)
            throw new ArgumentException("Denial ID must be greater than zero.");

        if (string.IsNullOrWhiteSpace(fileName))
            throw new ArgumentException("File name is required.");

        if (string.IsNullOrWhiteSpace(contentType))
            throw new ArgumentException("Content type is required.");

        if (fileSizeBytes <= 0)
            throw new ArgumentException("File size must be greater than zero.");

        if (string.IsNullOrWhiteSpace(storageReference))
            throw new ArgumentException("Storage reference is required.");

        if (uploadedByUserId.HasValue && uploadedByUserId.Value <= 0)
            throw new ArgumentException("User ID must be greater than zero.");

        TenantId = tenantId;
        DenialId = denialId;
        FileName = fileName.Trim();
        ContentType = contentType.Trim();
        FileSizeBytes = fileSizeBytes;
        StorageReference = storageReference.Trim();
        UploadedByUserId = uploadedByUserId;
        UploadedAtUtc = DateTime.UtcNow;
    }

    public long TenantId { get; private set; }

    public long DenialId { get; private set; }

    public string FileName { get; private set; } = string.Empty;

    public string ContentType { get; private set; } = string.Empty;

    public long FileSizeBytes { get; private set; }

    public string StorageReference { get; private set; } = string.Empty;

    public long? UploadedByUserId { get; private set; }

    public DateTime UploadedAtUtc { get; private set; }
}
