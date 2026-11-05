using InsureFlow.Domain.Common;
using InsureFlow.Domain.Enums;

namespace InsureFlow.Domain.Entities;

public sealed class Organization : AuditableEntity
{
    private Organization()
    {
    }

    public Organization(
        long tenantId,
        string code,
        string name,
        OrganizationType type = OrganizationType.MedicalGroup)
    {
        if (tenantId <= 0)
        {
            throw new ArgumentException(
                "Tenant ID is required.",
                nameof(tenantId));
        }

        if (string.IsNullOrWhiteSpace(code))
        {
            throw new ArgumentException(
                "Organization code is required.",
                nameof(code));
        }

        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException(
                "Organization name is required.",
                nameof(name));
        }

        TenantId = tenantId;
        Code = code.Trim().ToUpperInvariant();
        Name = name.Trim();
        Type = type;
        Status = RecordStatus.Active;
    }

    public long TenantId { get; private set; }

    public string Code { get; private set; } = null!;

    public string Name { get; private set; } = null!;

    public string? LegalName { get; private set; }

    public OrganizationType Type { get; private set; }

    public string? TaxId { get; private set; }

    public string? Npi { get; private set; }

    public string? Phone { get; private set; }

    public string? Fax { get; private set; }

    public string? Email { get; private set; }

    public string? Website { get; private set; }

    public RecordStatus Status { get; private set; }

    public Tenant Tenant { get; private set; } = null!;

    public ICollection<Provider> Providers { get; private set; }
        = new List<Provider>();

    public void UpdateDetails(
        string name,
        string? legalName,
        string? phone,
        string? fax,
        string? email,
        string? website)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException(
                "Organization name is required.",
                nameof(name));
        }

        Name = name.Trim();
        LegalName = NormalizeOptionalValue(legalName);
        Phone = NormalizeOptionalValue(phone);
        Fax = NormalizeOptionalValue(fax);
        Email = NormalizeOptionalValue(email);
        Website = NormalizeOptionalValue(website);

        UpdatedAtUtc = DateTime.UtcNow;
    }

    public void Activate()
    {
        Status = RecordStatus.Active;
        UpdatedAtUtc = DateTime.UtcNow;
    }

    public void Deactivate()
    {
        Status = RecordStatus.Inactive;
        UpdatedAtUtc = DateTime.UtcNow;
    }

    private static string? NormalizeOptionalValue(string? value)
    {
        return string.IsNullOrWhiteSpace(value)
            ? null
            : value.Trim();
    }
}
