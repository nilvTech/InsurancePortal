namespace InsureFlow.Contracts.Payers;

public sealed record PayerDto(
    Guid Id,
    long TenantId,
    string Code,
    string Name,
    string? PayerId,
    string Type,
    string? Phone,
    string? Website,
    string Status,
    DateTime CreatedAtUtc,
    DateTime? UpdatedAtUtc);