using InsureFlow.Domain.Common;
using InsureFlow.Domain.Enums;

namespace InsureFlow.Domain.Entities;

public sealed class PatientInsurance : AuditableEntity
{
    private PatientInsurance()
    {
    }

    public PatientInsurance(
        long tenantId,
        long patientId,
        long payerId,
        long? insurancePlanId,
        InsuranceType insuranceType,
        CoveragePriority coveragePriority,
        string memberId)
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

        if (payerId <= 0)
        {
            throw new ArgumentException(
                "Payer ID is required.",
                nameof(payerId));
        }

        if (string.IsNullOrWhiteSpace(memberId))
        {
            throw new ArgumentException(
                "Member ID is required.",
                nameof(memberId));
        }

        TenantId = tenantId;
        PatientId = patientId;
        PayerId = payerId;
        InsurancePlanId = insurancePlanId;

        InsuranceType = insuranceType;
        CoveragePriority = coveragePriority;

        InsuranceStatus = InsuranceStatus.PendingVerification;

        MemberId = memberId.Trim();

        SubscriberRelationship = SubscriberRelationship.Self;

        EligibilityStatus = EligibilityStatus.NotVerified;

        IsActive = true;
    }

    public long TenantId { get; private set; }

    public long PatientId { get; private set; }

    public long PayerId { get; private set; }

    public long? InsurancePlanId { get; private set; }

    public InsuranceType InsuranceType { get; private set; }

    public CoveragePriority CoveragePriority { get; private set; }

    public InsuranceStatus InsuranceStatus { get; private set; }

    public string MemberId { get; private set; } = null!;

    public string? GroupNumber { get; private set; }

    public string? SubscriberId { get; private set; }

    public string? SubscriberFirstName { get; private set; }

    public string? SubscriberLastName { get; private set; }

    public SubscriberRelationship SubscriberRelationship { get; private set; }

    public DateTime? EffectiveDate { get; private set; }

    public DateTime? TerminationDate { get; private set; }

    public string? PayerIdDisplay { get; private set; }

    public EligibilityStatus EligibilityStatus { get; private set; }

    public DateTime? LastVerifiedAt { get; private set; }

    public bool IsActive { get; private set; }

    public Tenant Tenant { get; private set; } = null!;

    public Patient Patient { get; private set; } = null!;

    public Payer Payer { get; private set; } = null!;

    public InsurancePlan? InsurancePlan { get; private set; }

    public void UpdateDetails(
        long payerId,
        long? insurancePlanId,
        InsuranceType insuranceType,
        CoveragePriority coveragePriority,
        InsuranceStatus insuranceStatus,
        string memberId,
        string? groupNumber,
        string? subscriberId,
        string? subscriberFirstName,
        string? subscriberLastName,
        SubscriberRelationship subscriberRelationship,
        DateTime? effectiveDate,
        DateTime? terminationDate,
        string? payerIdDisplay)
    {
        if (payerId <= 0)
        {
            throw new ArgumentException(
                "Payer ID is required.",
                nameof(payerId));
        }

        if (string.IsNullOrWhiteSpace(memberId))
        {
            throw new ArgumentException(
                "Member ID is required.",
                nameof(memberId));
        }

        if (effectiveDate.HasValue &&
            terminationDate.HasValue &&
            terminationDate.Value < effectiveDate.Value)
        {
            throw new ArgumentException(
                "Termination date cannot be earlier than effective date.",
                nameof(terminationDate));
        }

        PayerId = payerId;
        InsurancePlanId = insurancePlanId;

        InsuranceType = insuranceType;
        CoveragePriority = coveragePriority;
        InsuranceStatus = insuranceStatus;

        MemberId = memberId.Trim();

        GroupNumber = NormalizeOptional(groupNumber);
        SubscriberId = NormalizeOptional(subscriberId);
        SubscriberFirstName = NormalizeOptional(subscriberFirstName);
        SubscriberLastName = NormalizeOptional(subscriberLastName);

        SubscriberRelationship = subscriberRelationship;

        EffectiveDate = effectiveDate;
        TerminationDate = terminationDate;

        PayerIdDisplay = NormalizeOptional(payerIdDisplay);

        UpdatedAtUtc = DateTime.UtcNow;
    }

    public void UpdateSubscriber(
        string? subscriberId,
        string? subscriberFirstName,
        string? subscriberLastName,
        SubscriberRelationship relationship)
    {
        SubscriberId = NormalizeOptional(subscriberId);
        SubscriberFirstName = NormalizeOptional(subscriberFirstName);
        SubscriberLastName = NormalizeOptional(subscriberLastName);
        SubscriberRelationship = relationship;

        UpdatedAtUtc = DateTime.UtcNow;
    }

    public void MarkEligibilityVerified(DateTime verifiedAtUtc)
    {
        EligibilityStatus = EligibilityStatus.Verified;
        LastVerifiedAt = verifiedAtUtc;
        InsuranceStatus = InsuranceStatus.Active;
        IsActive = true;

        UpdatedAtUtc = DateTime.UtcNow;
    }

    public void MarkEligibilityInactive(DateTime verifiedAtUtc)
    {
        EligibilityStatus = EligibilityStatus.Inactive;
        LastVerifiedAt = verifiedAtUtc;
        InsuranceStatus = InsuranceStatus.Inactive;
        IsActive = false;

        UpdatedAtUtc = DateTime.UtcNow;
    }

    public void MarkEligibilityError(DateTime verifiedAtUtc)
    {
        EligibilityStatus = EligibilityStatus.Error;
        LastVerifiedAt = verifiedAtUtc;

        UpdatedAtUtc = DateTime.UtcNow;
    }

    public void Activate()
    {
        InsuranceStatus = InsuranceStatus.Active;
        IsActive = true;

        UpdatedAtUtc = DateTime.UtcNow;
    }

    public void Deactivate()
    {
        InsuranceStatus = InsuranceStatus.Inactive;
        IsActive = false;

        UpdatedAtUtc = DateTime.UtcNow;
    }

    public void Terminate(DateTime? terminationDate = null)
    {
        InsuranceStatus = InsuranceStatus.Terminated;
        IsActive = false;

        TerminationDate = terminationDate ?? DateTime.UtcNow.Date;

        UpdatedAtUtc = DateTime.UtcNow;
    }

    public void Expire()
    {
        InsuranceStatus = InsuranceStatus.Expired;
        IsActive = false;

        UpdatedAtUtc = DateTime.UtcNow;
    }

    private static string? NormalizeOptional(string? value)
    {
        return string.IsNullOrWhiteSpace(value)
            ? null
            : value.Trim();
    }
}