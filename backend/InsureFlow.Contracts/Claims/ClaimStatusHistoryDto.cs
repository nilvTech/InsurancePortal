
namespace InsureFlow.Contracts.Claims;

public sealed record ClaimStatusHistoryDto(
    long Id,
    string FromStatus,
    string ToStatus,
    string? Note,
    DateTime ChangedAtUtc,
    long? ChangedByUserId);
