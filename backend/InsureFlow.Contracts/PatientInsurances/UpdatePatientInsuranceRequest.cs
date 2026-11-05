namespace InsureFlow.Contracts.PatientInsurances;

public sealed record UpdatePatientInsuranceRequest(
    Guid PayerId,
    Guid? InsurancePlanId,
    string InsuranceType,
    string CoveragePriority,
    string InsuranceStatus,
    string MemberId,
    string? GroupNumber,
    string? SubscriberId,
    string? SubscriberFirstName,
    string? SubscriberLastName,
    string SubscriberRelationship,
    DateTime? EffectiveDate,
    DateTime? TerminationDate,
    string? PayerIdDisplay);