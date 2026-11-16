
namespace InsureFlow.Contracts.Appeals;

public sealed record AppealDocumentationRequest(
    string DocumentTitle,
    string DocumentType,
    string? Description,
    string? StorageReference);
