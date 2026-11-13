namespace InsureFlow.Contracts.Denials;

public sealed record CreateDenialReasonRequest(
    string ReasonCode,
    string ReasonName,
    string? Description,
    string Category);