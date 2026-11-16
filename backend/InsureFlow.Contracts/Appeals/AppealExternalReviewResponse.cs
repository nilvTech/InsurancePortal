
namespace InsureFlow.Contracts.Appeals;

public sealed record AppealExternalReviewResponse(
    Guid ExternalId,
    Guid AppealExternalId,
    string ReviewingOrganization,
    string? ExternalReferenceNumber,
    string Status,
    DateTime RequestedAtUtc,
    DateTime? ReviewDueAtUtc,
    DateTime? RespondedAtUtc,
    string? ReviewNotes);
