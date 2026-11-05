namespace InsureFlow.Contracts.Tenants;

public sealed record UpdateTenantRequest
{
    public string Name { get; init; } = string.Empty;

    public string? LegalName { get; init; }

    public string? ContactEmail { get; init; }

    public string? ContactPhone { get; init; }

    public string? TimeZone { get; init; }

    public string? DefaultCurrency { get; init; }
}

