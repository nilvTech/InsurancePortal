
namespace InsureFlow.Contracts.Denials;

public sealed record DenialActivityResponse(
    Guid ExternalId,
    string ActivityType,
    string Description,
    long? PerformedByUserId,
    DateTime ActivityDateUtc,
    DateTime CreatedAtUtc);
