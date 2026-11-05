namespace InsureFlow.Contracts.PatientInsurances;

public sealed record PatientInsuranceListRequest(
    string? Search = null,
    string? Status = null,
    string? InsuranceType = null,
    string? CoveragePriority = null,
    string? EligibilityStatus = null,
    int Page = 1,
    int PageSize = 25);