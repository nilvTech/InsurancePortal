
namespace InsureFlow.Contracts.Appeals;

public sealed record RecordAppealDecisionRequest(
    string Decision,
    string? DecisionNotes);
