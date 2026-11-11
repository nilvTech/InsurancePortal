
namespace InsureFlow.Contracts.Denials;

public sealed record DenialActivityRequest(
    string ActivityType,
    string Description);
