
namespace InsureFlow.Contracts.Appeals;

public sealed record AppealResponse(
    Guid ExternalId,
    Guid DenialExternalId,
    string AppealNumber,
    string AppealLevel,
    string AppealReason,
    string? AppealNotes,
    string Status,
    DateTime? AppealDeadlineUtc,
    DateTime? SubmittedAtUtc,
    DateTime? WithdrawnAtUtc,
    string? Decision,
    string? DecisionNotes,
    DateTime? DecisionAtUtc,
    DateTime CreatedAtUtc,
    DateTime? UpdatedAtUtc);
