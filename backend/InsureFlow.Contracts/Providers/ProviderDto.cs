namespace InsureFlow.Contracts.Providers;

public sealed record ProviderDto(
    Guid Id,
    long TenantId,
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
    string? Email,
    string Status,
    DateTime CreatedAtUtc,
    DateTime? UpdatedAtUtc);