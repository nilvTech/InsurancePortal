
namespace InsureFlow.Domain.Entities;

public sealed class AuthorizationServiceLine
{
    private AuthorizationServiceLine()
    {
    }

    public AuthorizationServiceLine(
        long tenantId,
        long authorizationId,
        string cptHcpcsCode,
        int unitsRequested)
    {
        if (tenantId <= 0)
            throw new ArgumentOutOfRangeException(nameof(tenantId));

        if (authorizationId <= 0)
            throw new ArgumentOutOfRangeException(nameof(authorizationId));

        if (string.IsNullOrWhiteSpace(cptHcpcsCode))
            throw new ArgumentException(
                "CPT/HCPCS code is required.",
                nameof(cptHcpcsCode));

        if (unitsRequested < 1)
            throw new ArgumentOutOfRangeException(
                nameof(unitsRequested),
                "Units requested must be at least one.");

        TenantId = tenantId;
        AuthorizationId = authorizationId;
        CptHcpcsCode = cptHcpcsCode.Trim().ToUpperInvariant();
        UnitsRequested = unitsRequested;
        Status = "REQUESTED";
    }

    public long Id { get; private set; }

    public long TenantId { get; private set; }

    public long AuthorizationId { get; private set; }

    public string CptHcpcsCode { get; private set; } = string.Empty;

    public string? Modifier { get; private set; }

    public string? DiagnosisCode { get; private set; }

    public int UnitsRequested { get; private set; }

    public int? UnitsApproved { get; private set; }

    public decimal? RequestedAmount { get; private set; }

    public decimal? ApprovedAmount { get; private set; }

    public string Status { get; private set; } = "REQUESTED";

    public void Update(
        int? unitsRequested,
        int? unitsApproved,
        decimal? requestedAmount,
        decimal? approvedAmount,
        string? status)
    {
        if (unitsRequested.HasValue && unitsRequested.Value < 1)
            throw new ArgumentOutOfRangeException(
                nameof(unitsRequested));

        if (unitsApproved.HasValue && unitsApproved.Value < 0)
            throw new ArgumentOutOfRangeException(
                nameof(unitsApproved));

        if (requestedAmount.HasValue && requestedAmount.Value < 0)
            throw new ArgumentOutOfRangeException(
                nameof(requestedAmount));

        if (approvedAmount.HasValue && approvedAmount.Value < 0)
            throw new ArgumentOutOfRangeException(
                nameof(approvedAmount));

        if (!string.IsNullOrWhiteSpace(status))
            Status = status.Trim().ToUpperInvariant();

        if (unitsRequested.HasValue)
            UnitsRequested = unitsRequested.Value;

        if (unitsApproved.HasValue)
            UnitsApproved = unitsApproved.Value;

        if (requestedAmount.HasValue)
            RequestedAmount = requestedAmount.Value;

        if (approvedAmount.HasValue)
            ApprovedAmount = approvedAmount.Value;
    }
}