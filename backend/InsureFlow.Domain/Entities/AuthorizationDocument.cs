namespace InsureFlow.Domain.Entities;

/// <summary>Tenant-scoped metadata for a document already stored in secure object storage.</summary>
public sealed class AuthorizationDocument
{
    private AuthorizationDocument() { }

    public AuthorizationDocument(
        long tenantId,
        long authorizationId,
        string fileName,
        string storageKey,
        string? contentType,
        long? fileSize,
        string? documentType)
    {
        if (tenantId <= 0 || authorizationId <= 0)
            throw new ArgumentOutOfRangeException(nameof(tenantId));

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
        AuthorizationId = authorizationId;
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

    public long AuthorizationId { get; private set; }

    public string FileName { get; private set; } = null!;

    public string StorageKey { get; private set; } = null!;

    public string? ContentType { get; private set; }

    public long? FileSize { get; private set; }

    public string DocumentType { get; private set; } = "SUPPORTING";

    public DateTime UploadedAt { get; private set; }
}