namespace InsureFlow.Contracts.Providers;

public sealed record CreateProviderRequest(
    Guid OrganizationId,
    string Code,
    string FirstName,
    string? MiddleName,
    string LastName,
    string? Credential,
    string Npi,
    string? Specialty,
    string? Department,
    string? Phone,
    string? Email);