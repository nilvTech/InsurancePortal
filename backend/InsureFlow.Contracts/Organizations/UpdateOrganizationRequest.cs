namespace InsureFlow.Contracts.Organizations;

public sealed record UpdateOrganizationRequest(
    string Name,
    string? LegalName,
    string? Phone,
    string? Fax,
    string? Email,
    string? Website);