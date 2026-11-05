using InsureFlow.Domain.Common;
using InsureFlow.Domain.Enums;

namespace InsureFlow.Domain.Entities;

public sealed class Payer : AuditableEntity
{
    private Payer()
    {
    }

    public Payer(
        long tenantId,
        string code,
        string name,
        PayerType type)
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
                "Payer code is required.",
                nameof(code));
        }

        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException(
                "Payer name is required.",
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

    public string? PayerId { get; private set; }

    public PayerType Type { get; private set; }

    public string? Phone { get; private set; }

    public string? Website { get; private set; }

    public RecordStatus Status { get; private set; }

    public Tenant Tenant { get; private set; } = null!;

    public void UpdateDetails(
        string code,
        string name,
        string? payerId,
        PayerType type,
        string? phone,
        string? website)
    {
        if (string.IsNullOrWhiteSpace(code))
        {
            throw new ArgumentException(
                "Payer code is required.",
                nameof(code));
        }

        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException(
                "Payer name is required.",
                nameof(name));
        }

        Code = code.Trim().ToUpperInvariant();
        Name = name.Trim();
        PayerId = NormalizeOptional(payerId);
        Type = type;
        Phone = NormalizeOptional(phone);
        Website = NormalizeOptional(website);

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

    private static string? NormalizeOptional(string? value)
    {
        return string.IsNullOrWhiteSpace(value)
            ? null
            : value.Trim();
    }
}