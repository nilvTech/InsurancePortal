namespace InsureFlow.Contracts.Payers;

public sealed record CreatePayerRequest(
    string Code,
    string Name,
    string? PayerId,
    string Type,
    string? Phone,
    string? Website);