namespace InsureFlow.Contracts.Denials;

public sealed record UpdateDenialReasonRequest(
    string ReasonName,
    string? Description,
    string Category,
    bool IsActive);