using InsureFlow.Domain.Common;
using InsureFlow.Domain.Enums;

namespace InsureFlow.Domain.Entities;

public sealed class Patient : AuditableEntity
{
    private Patient()
    {
    }

    public Patient(
        long tenantId,
        string patientId,
        string mrn,
        string accountNumber,
        string firstName,
        string lastName,
        DateTime dateOfBirth)
    {
        if (tenantId <= 0)
        {
            throw new ArgumentException(
                "Tenant ID is required.",
                nameof(tenantId));
        }

        if (string.IsNullOrWhiteSpace(patientId))
        {
            throw new ArgumentException(
                "Patient ID is required.",
                nameof(patientId));
        }

        if (string.IsNullOrWhiteSpace(mrn))
        {
            throw new ArgumentException(
                "MRN is required.",
                nameof(mrn));
        }

        if (string.IsNullOrWhiteSpace(accountNumber))
        {
            throw new ArgumentException(
                "Account number is required.",
                nameof(accountNumber));
        }

        if (string.IsNullOrWhiteSpace(firstName))
        {
            throw new ArgumentException(
                "Patient first name is required.",
                nameof(firstName));
        }

        if (string.IsNullOrWhiteSpace(lastName))
        {
            throw new ArgumentException(
                "Patient last name is required.",
                nameof(lastName));
        }

        if (dateOfBirth == default)
        {
            throw new ArgumentException(
                "Date of birth is required.",
                nameof(dateOfBirth));
        }

        TenantId = tenantId;
        PatientId = patientId.Trim();
        Mrn = mrn.Trim();
        AccountNumber = accountNumber.Trim();
        FirstName = firstName.Trim();
        LastName = lastName.Trim();
        DateOfBirth = dateOfBirth.Date;

        Gender = PatientGender.Unknown;
        Status = PatientStatus.Active;
        PatientClass = InsureFlow.Domain.Enums.PatientClass.Outpatient;
        PortalAccess = PortalAccessStatus.Disabled;
        CommunicationConsent = CommunicationConsentStatus.Pending;
        FinancialAssistance = FinancialAssistanceStatus.None;
    }

    public long TenantId { get; private set; }

    public string PatientId { get; private set; } = null!;

    public string Mrn { get; private set; } = null!;

    public string AccountNumber { get; private set; } = null!;

    public string FirstName { get; private set; } = null!;

    public string? MiddleName { get; private set; }

    public string LastName { get; private set; } = null!;

    public string? PreferredName { get; private set; }

    public DateTime DateOfBirth { get; private set; }

    public PatientGender Gender { get; private set; }

    public PatientStatus Status { get; private set; }

    public DateTime? RegistrationDate { get; private set; }

    public DateTime? LastVisitDate { get; private set; }

    public PatientClass? PatientClass { get; private set; }

    public PortalAccessStatus PortalAccess { get; private set; }

    public CommunicationConsentStatus CommunicationConsent { get; private set; }

    public string? PreferredPharmacy { get; private set; }

    public ReferralSource? ReferralSource { get; private set; }

    public FinancialAssistanceStatus FinancialAssistance { get; private set; }

    public string? Notes { get; private set; }

    public Tenant Tenant { get; private set; } = null!;

    public void UpdateDemographics(
        string firstName,
        string? middleName,
        string lastName,
        string? preferredName,
        DateTime dateOfBirth,
        PatientGender gender)
    {
        if (string.IsNullOrWhiteSpace(firstName))
        {
            throw new ArgumentException(
                "Patient first name is required.",
                nameof(firstName));
        }

        if (string.IsNullOrWhiteSpace(lastName))
        {
            throw new ArgumentException(
                "Patient last name is required.",
                nameof(lastName));
        }

        if (dateOfBirth == default)
        {
            throw new ArgumentException(
                "Date of birth is required.",
                nameof(dateOfBirth));
        }

        FirstName = firstName.Trim();
        MiddleName = NormalizeOptionalValue(middleName);
        LastName = lastName.Trim();
        PreferredName = NormalizeOptionalValue(preferredName);
        DateOfBirth = dateOfBirth.Date;
        Gender = gender;

        UpdatedAtUtc = DateTime.UtcNow;
    }

    public void UpdateIdentifiers(
        string patientId,
        string mrn,
        string accountNumber)
    {
        if (string.IsNullOrWhiteSpace(patientId))
        {
            throw new ArgumentException(
                "Patient ID is required.",
                nameof(patientId));
        }

        if (string.IsNullOrWhiteSpace(mrn))
        {
            throw new ArgumentException(
                "MRN is required.",
                nameof(mrn));
        }

        if (string.IsNullOrWhiteSpace(accountNumber))
        {
            throw new ArgumentException(
                "Account number is required.",
                nameof(accountNumber));
        }

        PatientId = patientId.Trim();
        Mrn = mrn.Trim();
        AccountNumber = accountNumber.Trim();

        UpdatedAtUtc = DateTime.UtcNow;
    }

    public void UpdatePatientInformation(
     DateTime? registrationDate,
     DateTime? lastVisitDate,
     PatientClass? patientClass,
     string? preferredPharmacy,
     ReferralSource? referralSource,
     string? notes)
    {
        RegistrationDate = registrationDate?.Date;
        LastVisitDate = lastVisitDate?.Date;
        PatientClass = patientClass;
        PreferredPharmacy = NormalizeOptionalValue(preferredPharmacy);
        ReferralSource = referralSource;
        Notes = NormalizeOptionalValue(notes);

        UpdatedAtUtc = DateTime.UtcNow;
    }

    public void EnablePortalAccess()
    {
        PortalAccess = PortalAccessStatus.Enabled;
        UpdatedAtUtc = DateTime.UtcNow;
    }

    public void DisablePortalAccess()
    {
        PortalAccess = PortalAccessStatus.Disabled;
        UpdatedAtUtc = DateTime.UtcNow;
    }

    public void SetPortalAccessPending()
    {
        PortalAccess = PortalAccessStatus.Pending;
        UpdatedAtUtc = DateTime.UtcNow;
    }

    public void GrantCommunicationConsent()
    {
        CommunicationConsent = CommunicationConsentStatus.Granted;
        UpdatedAtUtc = DateTime.UtcNow;
    }

    public void DeclineCommunicationConsent()
    {
        CommunicationConsent = CommunicationConsentStatus.Declined;
        UpdatedAtUtc = DateTime.UtcNow;
    }

    public void SetCommunicationConsentPending()
    {
        CommunicationConsent = CommunicationConsentStatus.Pending;
        UpdatedAtUtc = DateTime.UtcNow;
    }

    public void SetFinancialAssistance(
        FinancialAssistanceStatus status)
    {
        FinancialAssistance = status;
        UpdatedAtUtc = DateTime.UtcNow;
    }

    public void Activate()
    {
        Status = PatientStatus.Active;
        UpdatedAtUtc = DateTime.UtcNow;
    }

    public void Deactivate()
    {
        Status = PatientStatus.Inactive;
        UpdatedAtUtc = DateTime.UtcNow;
    }

    public void MarkDeceased()
    {
        Status = PatientStatus.Deceased;
        UpdatedAtUtc = DateTime.UtcNow;
    }

    public void MarkMerged()
    {
        Status = PatientStatus.Merged;
        UpdatedAtUtc = DateTime.UtcNow;
    }

    private static string? NormalizeOptionalValue(string? value)
    {
        return string.IsNullOrWhiteSpace(value)
            ? null
            : value.Trim();
    }
}