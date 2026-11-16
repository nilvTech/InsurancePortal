
namespace InsureFlow.Contracts.Appeals;

public sealed record CreateAppealRequest(
    Guid DenialExternalId,
    string AppealNumber,
    string AppealLevel,
    string AppealReason,
    string? AppealNotes,
    DateTime? AppealDeadlineUtc);
