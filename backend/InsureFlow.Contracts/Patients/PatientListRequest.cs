namespace InsureFlow.Contracts.Patients;

public sealed record PatientListRequest(
    string? Search = null,
    string? Status = null,
    string? Gender = null,
    string? PatientClass = null,
    int Page = 1,
    int PageSize = 25);