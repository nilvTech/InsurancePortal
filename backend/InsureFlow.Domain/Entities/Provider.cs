using InsureFlow.Domain.Common;
using InsureFlow.Domain.Enums;

namespace InsureFlow.Domain.Entities;

public sealed class Provider : AuditableEntity
{
    private Provider()
    {
    }

    public Provider(
        long tenantId,
        long organizationId,
        string code,
        string firstName,
        string lastName,
        string npi)
    {
        if (tenantId <= 0)
        {
            throw new ArgumentException(
                "Tenant ID is required.",
                nameof(tenantId));
        }

        if (organizationId <= 0)
        {
            throw new ArgumentException(
                "Organization ID is required.",
                nameof(organizationId));
        }

        if (string.IsNullOrWhiteSpace(code))
        {
            throw new ArgumentException(
                "Provider code is required.",
                nameof(code));
        }

        if (string.IsNullOrWhiteSpace(firstName))
        {
            throw new ArgumentException(
                "Provider first name is required.",
                nameof(firstName));
        }

        if (string.IsNullOrWhiteSpace(lastName))
        {
            throw new ArgumentException(
                "Provider last name is required.",
                nameof(lastName));
        }

        if (string.IsNullOrWhiteSpace(npi))
        {
            throw new ArgumentException(
                "Provider NPI is required.",
                nameof(npi));
        }

        TenantId = tenantId;
        OrganizationId = organizationId;
        Code = code.Trim().ToUpperInvariant();
        FirstName = firstName.Trim();
        LastName = lastName.Trim();
        Npi = npi.Trim();
        Status = RecordStatus.Active;
    }

    public long TenantId { get; private set; }

    public long OrganizationId { get; private set; }

    public string Code { get; private set; } = null!;

    public string FirstName { get; private set; } = null!;

    public string? MiddleName { get; private set; }

    public string LastName { get; private set; } = null!;

    public string? Credential { get; private set; }

    public string Npi { get; private set; } = null!;

    public string? Specialty { get; private set; }

    public string? Department { get; private set; }

    public string? Phone { get; private set; }

    public string? Email { get; private set; }

    public RecordStatus Status { get; private set; }

    public Tenant Tenant { get; private set; } = null!;

    public Organization Organization { get; private set; } = null!;

    public void UpdateDetails(
        string firstName,
        string? middleName,
        string lastName,
        string? credential,
        string? specialty,
        string? department,
        string? phone,
        string? email)
    {
        if (string.IsNullOrWhiteSpace(firstName))
        {
            throw new ArgumentException(
                "Provider first name is required.",
                nameof(firstName));
        }

        if (string.IsNullOrWhiteSpace(lastName))
        {
            throw new ArgumentException(
                "Provider last name is required.",
                nameof(lastName));
        }

        FirstName = firstName.Trim();
        MiddleName = NormalizeOptionalValue(middleName);
        LastName = lastName.Trim();
        Credential = NormalizeOptionalValue(credential);
        Specialty = NormalizeOptionalValue(specialty);
        Department = NormalizeOptionalValue(department);
        Phone = NormalizeOptionalValue(phone);
        Email = NormalizeOptionalValue(email);

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
