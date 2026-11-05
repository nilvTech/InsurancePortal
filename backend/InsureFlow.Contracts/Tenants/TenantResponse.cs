using InsureFlow.Domain.Enums;

namespace InsureFlow.Contracts.Tenants;

public sealed record TenantResponse
{
    public Guid Id { get; init; }

    public string Name { get; init; } = string.Empty;

    public string Code { get; init; } = string.Empty;

    public string? LegalName { get; init; }

    public string? TaxIdentifierLastFour { get; init; }

    public TenantStatus Status { get; init; }

    public string? TimeZone { get; init; }

    public string? DefaultCurrency { get; init; }

    public string? ContactEmail { get; init; }

    public string? ContactPhone { get; init; }

    public DateTime CreatedAtUtc { get; init; }

    public DateTime? UpdatedAtUtc { get; init; }
}

