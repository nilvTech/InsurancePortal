namespace InsureFlow.Contracts.Organizations;

public sealed record OrganizationDto(
    Guid Id,
    long TenantId,
    string Code,
    string Name,
    string? LegalName,
    string Type,
    string? TaxId,
    string? Npi,
    string? Phone,
    string? Fax,
    string? Email,
    string? Website,
    string Status,
    DateTime CreatedAtUtc,
    DateTime? UpdatedAtUtc);