
using InsureFlow.Domain.Common;

namespace InsureFlow.Domain.Entities;

public sealed class Claim : AuditableEntity
{
    private Claim()
    {
    }

    public Claim(
        long tenantId,
        long patientId,
        long patientInsuranceId,
        long payerId,
        long providerId,
        string claimNumber,
        string claimType,
        DateTime serviceFrom,
        DateTime serviceTo)
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

        if (string.IsNullOrWhiteSpace(claimNumber))
            throw new ArgumentException(
                "Claim number is required.",
                nameof(claimNumber));

        if (string.IsNullOrWhiteSpace(claimType))
            throw new ArgumentException(
                "Claim type is required.",
                nameof(claimType));

        ValidateServiceDates(serviceFrom, serviceTo);

        TenantId = tenantId;
        PatientId = patientId;
        PatientInsuranceId = patientInsuranceId;
        PayerId = payerId;
        ProviderId = providerId;

        ClaimNumber = claimNumber.Trim();
        ClaimType = claimType.Trim().ToUpperInvariant();

        Status = "DRAFT";
        ServiceFrom = serviceFrom.Date;
        ServiceTo = serviceTo.Date;
        TotalChargeAmount = 0m;
    }

    public long TenantId { get; private set; }

    public long PatientId { get; private set; }

    public long PatientInsuranceId { get; private set; }

    public long PayerId { get; private set; }

    public long ProviderId { get; private set; }

    public string ClaimNumber { get; private set; } = string.Empty;

    public string ClaimType { get; private set; } = string.Empty;

    public string Status { get; private set; } = "DRAFT";

    public DateTime ServiceFrom { get; private set; }

    public DateTime ServiceTo { get; private set; }

    public decimal TotalChargeAmount { get; private set; }

    public string? PayerClaimNumber { get; private set; }

    public string? SubmissionReference { get; private set; }

    public DateTime? SubmittedAtUtc { get; private set; }

    public DateTime? LastStatusCheckedAtUtc { get; private set; }

    public string? Notes { get; private set; }

    public void UpdateDetails(
        long patientId,
        long patientInsuranceId,
        long payerId,
        long providerId,
        string claimType,
        DateTime serviceFrom,
        DateTime serviceTo,
        string? notes)
    {
        if (Status != "DRAFT")
            throw new InvalidOperationException(
                "Only draft claims can be edited.");

        if (patientId <= 0)
            throw new ArgumentOutOfRangeException(nameof(patientId));

        if (patientInsuranceId <= 0)
            throw new ArgumentOutOfRangeException(nameof(patientInsuranceId));

        if (payerId <= 0)
            throw new ArgumentOutOfRangeException(nameof(payerId));

        if (providerId <= 0)
            throw new ArgumentOutOfRangeException(nameof(providerId));

        if (string.IsNullOrWhiteSpace(claimType))
            throw new ArgumentException(
                "Claim type is required.",
                nameof(claimType));

        ValidateServiceDates(serviceFrom, serviceTo);

        PatientId = patientId;
        PatientInsuranceId = patientInsuranceId;
        PayerId = payerId;
        ProviderId = providerId;
        ClaimType = claimType.Trim().ToUpperInvariant();
        ServiceFrom = serviceFrom.Date;
        ServiceTo = serviceTo.Date;
        Notes = NormalizeOptional(notes);

        UpdatedAtUtc = DateTime.UtcNow;
    }

    public void UpdateTotalChargeAmount(decimal totalChargeAmount)
    {
        if (totalChargeAmount < 0)
            throw new ArgumentOutOfRangeException(nameof(totalChargeAmount));

        TotalChargeAmount = decimal.Round(
            totalChargeAmount,
            2,
            MidpointRounding.AwayFromZero);

        UpdatedAtUtc = DateTime.UtcNow;
    }

    public void ChangeStatus(string status)
    {
        if (string.IsNullOrWhiteSpace(status))
            throw new ArgumentException(
                "Claim status is required.",
                nameof(status));

        var normalized = status.Trim().ToUpperInvariant();

        var allowedStatuses = new[]
        {
            "DRAFT",
            "VALIDATION_FAILED",
            "READY_TO_SUBMIT",
            "SUBMITTED",
            "ACCEPTED",
            "REJECTED",
            "PENDING",
            "PARTIALLY_PAID",
            "PAID",
            "DENIED",
            "APPEALED",
            "CANCELLED",
            "RESUBMITTED"
        };

        if (!allowedStatuses.Contains(normalized))
            throw new ArgumentException(
                $"Unsupported claim status: {status}.",
                nameof(status));

        Status = normalized;
        UpdatedAtUtc = DateTime.UtcNow;
    }

    public void RecordSubmission(
        string? submissionReference,
        DateTime submittedAtUtc)
    {
        if (Status is not (
            "READY_TO_SUBMIT" or "REJECTED" or "RESUBMITTED"))
        {
            throw new InvalidOperationException(
                "The claim is not ready for submission.");
        }

        SubmissionReference = NormalizeOptional(submissionReference);
        SubmittedAtUtc = submittedAtUtc;
        Status = "SUBMITTED";
        UpdatedAtUtc = DateTime.UtcNow;
    }

    public void RecordPayerResponse(
        string? payerClaimNumber,
        string responseStatus)
    {
        if (string.IsNullOrWhiteSpace(responseStatus))
            throw new ArgumentException(
                "Response status is required.",
                nameof(responseStatus));

        var normalized = responseStatus.Trim().ToUpperInvariant();

        if (normalized is not (
            "ACCEPTED" or "REJECTED" or "PENDING" or "DENIED"))
        {
            throw new ArgumentException(
                $"Unsupported payer response status: {responseStatus}.",
                nameof(responseStatus));
        }

        PayerClaimNumber = NormalizeOptional(payerClaimNumber);
        Status = normalized;
        UpdatedAtUtc = DateTime.UtcNow;
    }

    public void RecordStatusCheck(DateTime checkedAtUtc)
    {
        LastStatusCheckedAtUtc = checkedAtUtc;
        UpdatedAtUtc = DateTime.UtcNow;
    }

    private static void ValidateServiceDates(
        DateTime serviceFrom,
        DateTime serviceTo)
    {
        if (serviceFrom == default)
            throw new ArgumentException(
                "Service start date is required.",
                nameof(serviceFrom));

        if (serviceTo == default)
            throw new ArgumentException(
                "Service end date is required.",
                nameof(serviceTo));

        if (serviceTo.Date < serviceFrom.Date)
            throw new ArgumentException(
                "Service end date cannot be earlier than start date.");
    }

    private static string? NormalizeOptional(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
