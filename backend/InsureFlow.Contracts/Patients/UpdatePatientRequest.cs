namespace InsureFlow.Contracts.Patients;

public sealed record UpdatePatientRequest(
    string PatientId,
    string Mrn,
    string AccountNumber,
    string FirstName,
    string? MiddleName,
    string LastName,
    string? PreferredName,
    DateTime DateOfBirth,
    string Gender,
    DateTime? RegistrationDate,
    DateTime? LastVisitDate,
    string PatientClass,
    string? PreferredPharmacy,
    string? ReferralSource,
    string? Notes);