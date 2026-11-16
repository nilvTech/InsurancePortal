
namespace InsureFlow.Contracts.Appeals;

public sealed record AppealDocumentationResponse(
    Guid ExternalId,
    Guid AppealExternalId,
    string DocumentTitle,
    string DocumentType,
    string? Description,
    string? StorageReference,
    DateTime CreatedAtUtc);
