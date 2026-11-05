namespace InsureFlow.Contracts.Claims;

public sealed record ClaimHistoryDto(
    long Id,
    string FromStatus,
    string ToStatus,
    string? Note,
    DateTime ChangedAt,
    long? ChangedByUserId);