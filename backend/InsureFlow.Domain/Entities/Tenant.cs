using InsureFlow.Domain.Common;
using InsureFlow.Domain.Enums;

namespace InsureFlow.Domain.Entities;

public sealed class Tenant : AuditableEntity
{
    private Tenant()
    {
    }

    public Tenant(
        string name,
        string code,
        string? legalName = null,
        string? taxIdentifierLastFour = null,
        string? contactFirstName = null,
        string? contactLastName = null,
        string? contactEmail = null,
        string? contactPhone = null)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException(
                "Tenant name is required.",
                nameof(name));
        }

        if (string.IsNullOrWhiteSpace(code))
        {
            throw new ArgumentException(
                "Tenant code is required.",
                nameof(code));
        }

        Name = name.Trim();
        Code = code.Trim().ToUpperInvariant();
        LegalName = NormalizeOptionalValue(legalName);
        TaxIdentifierLastFour =
            NormalizeOptionalValue(taxIdentifierLastFour);

        ContactFirstName = NormalizeOptionalValue(contactFirstName);
        ContactLastName = NormalizeOptionalValue(contactLastName);
        ContactEmail = NormalizeOptionalValue(contactEmail);
        ContactPhone = NormalizeOptionalValue(contactPhone);

        Status = TenantStatus.Active;
        SubscriptionPlan = SubscriptionPlan.Starter;
        TimeZone = "America/New_York";
    }

    public string Name { get; private set; } = null!;

    public string Code { get; private set; } = null!;

    public string? LegalName { get; private set; }

    public string? TaxIdentifierLastFour { get; private set; }

    public TenantStatus Status { get; private set; }

    public SubscriptionPlan SubscriptionPlan { get; private set; }

    public string? TimeZone { get; private set; }

    public string? DefaultCurrency { get; private set; }

    public string? ContactFirstName { get; private set; }

    public string? ContactLastName { get; private set; }

    public string? ContactEmail { get; private set; }

    public string? ContactPhone { get; private set; }

    public void UpdateDetails(
        string name,
        string? legalName,
        string? contactEmail,
        string? contactPhone)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException(
                "Tenant name is required.",
                nameof(name));
        }

        Name = name.Trim();
        LegalName = NormalizeOptionalValue(legalName);
        ContactEmail = NormalizeOptionalValue(contactEmail);
        ContactPhone = NormalizeOptionalValue(contactPhone);

        UpdatedAtUtc = DateTime.UtcNow;
    }

    public void UpdateContact(
        string? contactFirstName,
        string? contactLastName,
        string? contactEmail,
        string? contactPhone)
    {
        ContactFirstName = NormalizeOptionalValue(contactFirstName);
        ContactLastName = NormalizeOptionalValue(contactLastName);
        ContactEmail = NormalizeOptionalValue(contactEmail);
        ContactPhone = NormalizeOptionalValue(contactPhone);

        UpdatedAtUtc = DateTime.UtcNow;
    }

    public void ConfigureDefaults(
        string? timeZone,
        string? defaultCurrency)
    {
        TimeZone = NormalizeOptionalValue(timeZone);
        DefaultCurrency = NormalizeOptionalValue(defaultCurrency);

        UpdatedAtUtc = DateTime.UtcNow;
    }

    public void ChangeSubscriptionPlan(
        SubscriptionPlan subscriptionPlan)
    {
        SubscriptionPlan = subscriptionPlan;
        UpdatedAtUtc = DateTime.UtcNow;
    }

    public void Activate()
    {
        Status = TenantStatus.Active;
        UpdatedAtUtc = DateTime.UtcNow;
    }

    public void Deactivate()
    {
        Status = TenantStatus.Inactive;
        UpdatedAtUtc = DateTime.UtcNow;
    }

    public void Suspend()
    {
        Status = TenantStatus.Suspended;
        UpdatedAtUtc = DateTime.UtcNow;
    }

    public void StartTrial()
    {
        Status = TenantStatus.Trial;
        UpdatedAtUtc = DateTime.UtcNow;
    }

    private static string? NormalizeOptionalValue(string? value)
    {
        return string.IsNullOrWhiteSpace(value)
            ? null
            : value.Trim();
    }
}