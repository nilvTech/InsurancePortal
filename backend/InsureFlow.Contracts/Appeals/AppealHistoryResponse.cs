
namespace InsureFlow.Contracts.Appeals;

public sealed record AppealHistoryResponse(
    Guid ExternalId,
    Guid AppealExternalId,
    string ActivityType,
    string Description,
    long? PerformedByUserId,
    DateTime ActivityDateUtc);
