
namespace InsureFlow.Domain.Entities;

public sealed class Authorization
{
    private Authorization()
    {
    }

    public Authorization(
        long tenantId,
        long patientId,
        long patientInsuranceId,
        long payerId,
        long providerId,
        string authorizationNumber,
        DateTime? requestedFrom,
        DateTime? requestedTo)
    {
        if (tenantId <= 0)
            throw new ArgumentOutOfRangeException(nameof(tenantId));

        if (patientId <= 0)
            throw new ArgumentOutOfRangeException(nameof(patientId));

        if (patientInsuranceId <= 0)
            throw new ArgumentOutOfRangeException(nameof(patientInsuranceId));

        if (payerId <= 0)
            throw new ArgumentOutOfRangeException(nameof(payerId));

        if (providerId <= 0)
            throw new ArgumentOutOfRangeException(nameof(providerId));

        if (string.IsNullOrWhiteSpace(authorizationNumber))
            throw new ArgumentException(
                "Authorization number is required.",
                nameof(authorizationNumber));

        ValidateDates(requestedFrom, requestedTo);

        ExternalId = Guid.NewGuid();
        TenantId = tenantId;
        PatientId = patientId;
        PatientInsuranceId = patientInsuranceId;
        PayerId = payerId;
        ProviderId = providerId;
        AuthorizationNumber = authorizationNumber.Trim();

        RequestType = "STANDARD";
        Status = "DRAFT";
        Priority = "NORMAL";
        RequestedFrom = requestedFrom;
        RequestedTo = requestedTo;

        CreatedAtUtc = DateTime.UtcNow;
    }

    public long Id { get; private set; }

    public Guid ExternalId { get; private set; }

    public long TenantId { get; private set; }

    public long PatientId { get; private set; }

    public long PatientInsuranceId { get; private set; }

    public long PayerId { get; private set; }

    public long ProviderId { get; private set; }

    public string AuthorizationNumber { get; private set; } = string.Empty;

    public string RequestType { get; private set; } = "STANDARD";

    public string Status { get; private set; } = "DRAFT";

    public string Priority { get; private set; } = "NORMAL";

    public string? PlaceOfService { get; private set; }

    public DateTime? RequestedFrom { get; private set; }

    public DateTime? RequestedTo { get; private set; }

    public DateTime? DecisionDate { get; private set; }

    public string? PayerReference { get; private set; }

    public string? ClinicalSummary { get; private set; }

    public string? DecisionReason { get; private set; }

    public DateTime CreatedAtUtc { get; private set; }

    public DateTime? UpdatedAtUtc { get; private set; }

    public void UpdateDetails(
        string requestType,
        string priority,
        string? placeOfService,
        DateTime? requestedFrom,
        DateTime? requestedTo,
        string? clinicalSummary)
    {
        if (string.IsNullOrWhiteSpace(requestType))
            throw new ArgumentException(
                "Request type is required.",
                nameof(requestType));

        if (string.IsNullOrWhiteSpace(priority))
            throw new ArgumentException(
                "Priority is required.",
                nameof(priority));

        ValidateDates(requestedFrom, requestedTo);

        if (Status != "DRAFT")
            throw new InvalidOperationException(
                "Only draft authorizations can be edited.");

        RequestType = requestType.Trim().ToUpperInvariant();
        Priority = priority.Trim().ToUpperInvariant();

        PlaceOfService = string.IsNullOrWhiteSpace(placeOfService)
            ? null
            : placeOfService.Trim();

        RequestedFrom = requestedFrom;
        RequestedTo = requestedTo;

        ClinicalSummary = string.IsNullOrWhiteSpace(clinicalSummary)
            ? null
            : clinicalSummary.Trim();

        UpdatedAtUtc = DateTime.UtcNow;
    }

    public void ChangeStatus(string status, string? reason = null)
    {
        if (string.IsNullOrWhiteSpace(status))
            throw new ArgumentException(
                "Status is required.",
                nameof(status));

        var normalizedStatus = status.Trim().ToUpperInvariant();

        var allowedStatuses = new[]
        {
            "DRAFT",
            "PENDING",
            "APPROVED",
            "DENIED",
            "CANCELLED",
            "APPEALED"
        };

        if (!allowedStatuses.Contains(normalizedStatus))
            throw new ArgumentException(
                $"Unsupported authorization status: {status}.",
                nameof(status));

        Status = normalizedStatus;

        if (normalizedStatus is "APPROVED" or "DENIED")
            DecisionDate = DateTime.UtcNow;

        if (normalizedStatus == "DENIED")
        {
            DecisionReason = string.IsNullOrWhiteSpace(reason)
                ? null
                : reason.Trim();
        }

        UpdatedAtUtc = DateTime.UtcNow;
    }

    private static void ValidateDates(
        DateTime? requestedFrom,
        DateTime? requestedTo)
    {
        if (requestedFrom.HasValue &&
            requestedTo.HasValue &&
            requestedTo.Value < requestedFrom.Value)
        {
            throw new ArgumentException(
                "Requested end date cannot be earlier than the start date.");
        }
    }
}