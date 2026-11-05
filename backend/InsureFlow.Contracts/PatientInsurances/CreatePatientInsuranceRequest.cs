namespace InsureFlow.Contracts.PatientInsurances;

public sealed record CreatePatientInsuranceRequest(
    Guid PatientId,
    Guid PayerId,
    Guid? InsurancePlanId,
    string InsuranceType,
    string CoveragePriority,
    string MemberId,
    string? GroupNumber,
    string? SubscriberId,
    string? SubscriberFirstName,
    string? SubscriberLastName,
    string SubscriberRelationship,
    DateTime? EffectiveDate,
    DateTime? TerminationDate,
    string? PayerIdDisplay);