
namespace InsureFlow.Domain.Entities;

public sealed class ClaimDiagnosis
{
    private ClaimDiagnosis()
    {
        Icd10Code = string.Empty;
    }

    public ClaimDiagnosis(
        long tenantId,
        long claimId,
        string icd10Code,
        string? description,
        int sequence)
    {
        if (tenantId <= 0)
            throw new ArgumentOutOfRangeException(nameof(tenantId));

        if (claimId <= 0)
            throw new ArgumentOutOfRangeException(nameof(claimId));

        if (string.IsNullOrWhiteSpace(icd10Code))
            throw new ArgumentException(
                "ICD-10 diagnosis code is required.",
                nameof(icd10Code));

        if (sequence < 1)
            throw new ArgumentOutOfRangeException(nameof(sequence));

        TenantId = tenantId;
        ClaimId = claimId;
        Icd10Code = icd10Code.Trim().ToUpperInvariant();
        Description = string.IsNullOrWhiteSpace(description)
            ? null
            : description.Trim();
        Sequence = sequence;
    }

    public long Id { get; private set; }

    public long TenantId { get; private set; }

    public long ClaimId { get; private set; }

    public string Icd10Code { get; private set; }

    public string? Description { get; private set; }

    public int Sequence { get; private set; }
}
