
namespace InsureFlow.Contracts.Appeals;

public sealed record UpdateAppealRequest(
    string AppealLevel,
    string AppealReason,
    string? AppealNotes,
    DateTime? AppealDeadlineUtc);
