
namespace InsureFlow.Domain.Entities;

public sealed class AuthorizationDiagnosis
{
    // Required by Entity Framework Core.
    private AuthorizationDiagnosis()
    {
        Icd10Code = string.Empty;
    }

    public AuthorizationDiagnosis(
        long tenantId,
        long authorizationId,
        string icd10Code,
        string? description,
        int sequence)
    {
        if (tenantId <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(tenantId),
                "Tenant ID must be greater than zero.");
        }

        if (authorizationId <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(authorizationId),
                "Authorization ID must be greater than zero.");
        }

        if (string.IsNullOrWhiteSpace(icd10Code))
        {
            throw new ArgumentException(
                "ICD-10 diagnosis code is required.",
                nameof(icd10Code));
        }

        if (sequence < 1)
        {
            throw new ArgumentOutOfRangeException(
                nameof(sequence),
                "Diagnosis sequence must be at least 1.");
        }

        TenantId = tenantId;
        AuthorizationId = authorizationId;
        Icd10Code = icd10Code.Trim().ToUpperInvariant();
        Description = string.IsNullOrWhiteSpace(description)
            ? null
            : description.Trim();
        Sequence = sequence;
    }

    public long Id { get; private set; }

    public long TenantId { get; private set; }

    public long AuthorizationId { get; private set; }

    public string Icd10Code { get; private set; }

    public string? Description { get; private set; }

    public int Sequence { get; private set; }
}

