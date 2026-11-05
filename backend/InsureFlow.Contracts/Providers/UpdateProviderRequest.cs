namespace InsureFlow.Contracts.Providers;

public sealed record UpdateProviderRequest(
    string FirstName,
    string? MiddleName,
    string LastName,
    string? Credential,
    string? Specialty,
    string? Department,
    string? Phone,
    string? Email);