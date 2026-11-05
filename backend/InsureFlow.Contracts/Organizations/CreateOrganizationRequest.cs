namespace InsureFlow.Contracts.Organizations;

public sealed record CreateOrganizationRequest(
    string Code,
    string Name,
    string Type,
    string? LegalName,
    string? TaxId,
    string? Npi,
    string? Phone,
    string? Fax,
    string? Email,
    string? Website);