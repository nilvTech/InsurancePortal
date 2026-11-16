
using InsureFlow.Domain.Common;

namespace InsureFlow.Domain.Entities;

public sealed class AppealDocumentation : AuditableEntity
{
    private AppealDocumentation()
    {
    }

    public AppealDocumentation(
        long tenantId,
        long appealId,
        string documentTitle,
        string documentType,
        string? description = null,
        string? storageReference = null)
    {
        if (tenantId <= 0)
            throw new ArgumentOutOfRangeException(nameof(tenantId));

        if (appealId <= 0)
            throw new ArgumentOutOfRangeException(nameof(appealId));

        if (string.IsNullOrWhiteSpace(documentTitle))
            throw new ArgumentException(
                "Document title is required.",
                nameof(documentTitle));

        if (string.IsNullOrWhiteSpace(documentType))
            throw new ArgumentException(
                "Document type is required.",
                nameof(documentType));

        TenantId = tenantId;
        AppealId = appealId;
        DocumentTitle = documentTitle.Trim();
        DocumentType = documentType.Trim();
        Description = NormalizeOptionalText(description);
        StorageReference = NormalizeOptionalText(storageReference);
    }

    public long TenantId { get; private set; }

    public long AppealId { get; private set; }

    public string DocumentTitle { get; private set; } = string.Empty;

    public string DocumentType { get; private set; } = string.Empty;

    public string? Description { get; private set; }

    public string? StorageReference { get; private set; }

    private static string? NormalizeOptionalText(string? value)
    {
        return string.IsNullOrWhiteSpace(value)
            ? null
            : value.Trim();
    }
}
