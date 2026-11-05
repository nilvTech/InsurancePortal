namespace InsureFlow.Contracts.Claims;

public sealed record ClaimDiagnosisInput(
    string Icd10Code,
    string? Description,
    int Sequence);

public sealed record ClaimDiagnosisDto(
    long Id,
    string Icd10Code,
    string? Description,
    int Sequence);