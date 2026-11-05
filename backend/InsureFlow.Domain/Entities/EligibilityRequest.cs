using InsureFlow.Domain.Common;
using InsureFlow.Domain.Enums;

namespace InsureFlow.Domain.Entities;

public sealed class EligibilityRequest : AuditableEntity
{
    private EligibilityRequest()
    {
    }

    public EligibilityRequest(
        long tenantId,
        long patientId,
        long patientInsuranceId,
        long payerId,
        string requestNumber,
        EligibilityRequestType requestType,
        EligibilityRequestSource source,
        DateTime requestedAtUtc)
    {
        if (tenantId <= 0)
        {
            throw new ArgumentException(
                "Tenant ID is required.",
                nameof(tenantId));
        }

        if (patientId <= 0)
        {
            throw new ArgumentException(
                "Patient ID is required.",
                nameof(patientId));
        }

        if (patientInsuranceId <= 0)
        {
            throw new ArgumentException(
                "Patient insurance ID is required.",
                nameof(patientInsuranceId));
        }

        if (payerId <= 0)
        {
            throw new ArgumentException(
                "Payer ID is required.",
                nameof(payerId));
        }

        if (string.IsNullOrWhiteSpace(requestNumber))
        {
            throw new ArgumentException(
                "Request number is required.",
                nameof(requestNumber));
        }

        TenantId = tenantId;
        PatientId = patientId;
        PatientInsuranceId = patientInsuranceId;
        PayerId = payerId;

        RequestNumber = requestNumber.Trim();
        RequestType = requestType;
        Status = EligibilityRequestStatus.Pending;
        Source = source;

        RequestedAt = requestedAtUtc;

        CreatedAtUtc = requestedAtUtc;
        UpdatedAtUtc = requestedAtUtc;
    }

    public long TenantId { get; private set; }

    public long PatientId { get; private set; }

    public long PatientInsuranceId { get; private set; }

    public long PayerId { get; private set; }

    public string RequestNumber { get; private set; } = null!;

    public EligibilityRequestType RequestType { get; private set; }

    public EligibilityRequestStatus Status { get; private set; }

    public EligibilityRequestSource Source { get; private set; }

    public DateTime RequestedAt { get; private set; }

    public DateTime? CompletedAt { get; private set; }

    public string? ErrorCode { get; private set; }

    public string? ErrorMessage { get; private set; }

    public string? TraceId { get; private set; }

    public string? RawRequest { get; private set; }

    public Tenant Tenant { get; private set; } = null!;

    public Patient Patient { get; private set; } = null!;

    public PatientInsurance PatientInsurance { get; private set; } = null!;

    public Payer Payer { get; private set; } = null!;

    public EligibilityResponse? Response { get; private set; }

    public ICollection<EligibilityHistory> History { get; private set; }
        = new List<EligibilityHistory>();

    public void MarkProcessing()
    {
        Status = EligibilityRequestStatus.Processing;
        ErrorCode = null;
        ErrorMessage = null;
        UpdatedAtUtc = DateTime.UtcNow;
    }

    public void MarkCompleted(DateTime completedAtUtc)
    {
        Status = EligibilityRequestStatus.Completed;
        CompletedAt = completedAtUtc;
        ErrorCode = null;
        ErrorMessage = null;
        UpdatedAtUtc = completedAtUtc;
    }

    public void MarkFailed(
        string? errorCode,
        string? errorMessage,
        DateTime completedAtUtc)
    {
        Status = EligibilityRequestStatus.Failed;
        CompletedAt = completedAtUtc;
        ErrorCode = NormalizeOptional(errorCode);
        ErrorMessage = NormalizeOptional(errorMessage);
        UpdatedAtUtc = completedAtUtc;
    }

    public void Cancel()
    {
        Status = EligibilityRequestStatus.Cancelled;
        CompletedAt = DateTime.UtcNow;
        UpdatedAtUtc = DateTime.UtcNow;
    }

    public void SetTraceId(string? traceId)
    {
        TraceId = NormalizeOptional(traceId);
        UpdatedAtUtc = DateTime.UtcNow;
    }
    public void Retry()
    {
        if (Status != EligibilityRequestStatus.Failed)
        {
            throw new InvalidOperationException(
                "Only failed eligibility requests can be retried.");
        }

        Status = EligibilityRequestStatus.Pending;
        CompletedAt = null;
        ErrorCode = null;
        ErrorMessage = null;
    }
    public void SetRawRequest(string? rawRequest)
    {
        RawRequest = rawRequest;
        UpdatedAtUtc = DateTime.UtcNow;
    }

    private static string? NormalizeOptional(string? value)
    {
        return string.IsNullOrWhiteSpace(value)
            ? null
            : value.Trim();
    }
}