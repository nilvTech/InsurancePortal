namespace InsureFlow.Contracts.Payers;

public sealed record UpdatePayerRequest(
    string Code,
    string Name,
    string? PayerId,
    string Type,
    string? Phone,
    string? Website);