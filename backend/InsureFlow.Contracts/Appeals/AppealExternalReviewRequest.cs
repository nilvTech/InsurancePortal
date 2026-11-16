
namespace InsureFlow.Contracts.Appeals;

public sealed record AppealExternalReviewRequest(
    string ReviewingOrganization,
    string? ExternalReferenceNumber,
    DateTime? ReviewDueAtUtc,
    string? ReviewNotes);
