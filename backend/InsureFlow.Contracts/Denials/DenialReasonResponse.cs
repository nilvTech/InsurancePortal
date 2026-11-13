namespace InsureFlow.Contracts.Denials;

public sealed record DenialReasonResponse(
    Guid ExternalId,
    string ReasonCode,
    string ReasonName,
    string? Description,
    string Category,
    bool IsActive,
    DateTime CreatedAtUtc,
    DateTime? UpdatedAtUtc);