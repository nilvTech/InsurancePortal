
namespace InsureFlow.Domain.Entities;

/// <summary>
/// Tenant-scoped metadata for a claim document already stored
/// in approved secure object storage.
/// </summary>
public sealed class ClaimAttachment
{
    private ClaimAttachment()
    {
        FileName = string.Empty;
        StorageKey = string.Empty;
        DocumentType = "SUPPORTING";
    }

    public ClaimAttachment(
        long tenantId,
        long claimId,
        string fileName,
        string storageKey,
        string? contentType,
        long? fileSize,
        string? documentType)
    {
        if (tenantId <= 0)
            throw new ArgumentOutOfRangeException(nameof(tenantId));

        if (claimId <= 0)
            throw new ArgumentOutOfRangeException(nameof(claimId));

        if (string.IsNullOrWhiteSpace(fileName))
            throw new ArgumentException(
                "File name is required.",
                nameof(fileName));

        if (string.IsNullOrWhiteSpace(storageKey))
            throw new ArgumentException(
                "Storage key is required.",
                nameof(storageKey));

        if (fileSize < 0)
            throw new ArgumentOutOfRangeException(nameof(fileSize));

        TenantId = tenantId;
        ClaimId = claimId;
        FileName = fileName.Trim();
        StorageKey = storageKey.Trim();

        ContentType = string.IsNullOrWhiteSpace(contentType)
            ? null
            : contentType.Trim();

        FileSize = fileSize;

        DocumentType = string.IsNullOrWhiteSpace(documentType)
            ? "SUPPORTING"
            : documentType.Trim().ToUpperInvariant();

        UploadedAt = DateTime.UtcNow;
    }

    public long Id { get; private set; }

    public long TenantId { get; private set; }

    public long ClaimId { get; private set; }

    public string FileName { get; private set; }

    public string StorageKey { get; private set; }

    public string? ContentType { get; private set; }

    public long? FileSize { get; private set; }

    public string DocumentType { get; private set; }

    public DateTime UploadedAt { get; private set; }
}
