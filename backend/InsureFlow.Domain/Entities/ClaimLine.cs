
using InsureFlow.Domain.Common;

namespace InsureFlow.Domain.Entities;

public sealed class ClaimLine : AuditableEntity
{
    private ClaimLine()
    {
    }

    public ClaimLine(
        long tenantId,
        long claimId,
        int lineNumber,
        string procedureCode,
        decimal units,
        decimal chargeAmount,
        DateTime serviceDate)
    {
        if (tenantId <= 0)
            throw new ArgumentOutOfRangeException(nameof(tenantId));

        if (claimId <= 0)
            throw new ArgumentOutOfRangeException(nameof(claimId));

        if (lineNumber <= 0)
            throw new ArgumentOutOfRangeException(nameof(lineNumber));

        if (string.IsNullOrWhiteSpace(procedureCode))
            throw new ArgumentException(
                "Procedure code is required.",
                nameof(procedureCode));

        if (units <= 0)
            throw new ArgumentOutOfRangeException(nameof(units));

        if (chargeAmount < 0)
            throw new ArgumentOutOfRangeException(nameof(chargeAmount));

        if (serviceDate == default)
            throw new ArgumentException(
                "Service date is required.",
                nameof(serviceDate));

        TenantId = tenantId;
        ClaimId = claimId;
        LineNumber = lineNumber;
        ProcedureCode = procedureCode.Trim().ToUpperInvariant();
        Units = units;
        ChargeAmount = decimal.Round(
            chargeAmount, 2, MidpointRounding.AwayFromZero);
        ServiceDate = serviceDate.Date;
    }

    public long TenantId { get; private set; }

    public long ClaimId { get; private set; }

    public int LineNumber { get; private set; }

    public string ProcedureCode { get; private set; } = string.Empty;

    public string? Modifier1 { get; private set; }

    public string? Modifier2 { get; private set; }

    public string? Modifier3 { get; private set; }

    public string? Modifier4 { get; private set; }

    public string? PlaceOfService { get; private set; }

    public decimal Units { get; private set; }

    public decimal ChargeAmount { get; private set; }

    public DateTime ServiceDate { get; private set; }

    public string? DiagnosisPointers { get; private set; }

    public Claim Claim { get; private set; } = null!;

    public void UpdateDetails(
        string procedureCode,
        decimal units,
        decimal chargeAmount,
        DateTime serviceDate,
        string? modifier1,
        string? modifier2,
        string? modifier3,
        string? modifier4,
        string? placeOfService,
        string? diagnosisPointers)
    {
        if (string.IsNullOrWhiteSpace(procedureCode))
            throw new ArgumentException(
                "Procedure code is required.",
                nameof(procedureCode));

        if (units <= 0)
            throw new ArgumentOutOfRangeException(nameof(units));

        if (chargeAmount < 0)
            throw new ArgumentOutOfRangeException(nameof(chargeAmount));

        if (serviceDate == default)
            throw new ArgumentException(
                "Service date is required.",
                nameof(serviceDate));

        ProcedureCode = procedureCode.Trim().ToUpperInvariant();
        Units = units;
        ChargeAmount = decimal.Round(
            chargeAmount, 2, MidpointRounding.AwayFromZero);
        ServiceDate = serviceDate.Date;

        Modifier1 = NormalizeOptional(modifier1);
        Modifier2 = NormalizeOptional(modifier2);
        Modifier3 = NormalizeOptional(modifier3);
        Modifier4 = NormalizeOptional(modifier4);
        PlaceOfService = NormalizeOptional(placeOfService);
        DiagnosisPointers = NormalizeOptional(diagnosisPointers);

        UpdatedAtUtc = DateTime.UtcNow;
    }

    private static string? NormalizeOptional(string? value)
    {
        return string.IsNullOrWhiteSpace(value)
            ? null
            : value.Trim();
    }
}
