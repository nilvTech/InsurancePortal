
namespace InsureFlow.Contracts.Denials;

public sealed record DenialResponse(
    Guid ExternalId,
    Guid ClaimExternalId,
    string DenialNumber,
    string DenialCode,
    string DenialReason,
    string DenialCategory,
    decimal DeniedAmount,
    DateTime DenialDate,
    DateTime? AppealDeadline,
    string Status,
    long? AssignedToUserId,
    DateTime CreatedAtUtc,
    DateTime? UpdatedAtUtc);
