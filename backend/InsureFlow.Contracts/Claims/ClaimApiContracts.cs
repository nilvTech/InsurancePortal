namespace InsureFlow.Contracts.Claims;

public sealed record ClaimListRequest(
    string? Search = null,
    string? Status = null,
    string? ClaimType = null,
    Guid? PatientId = null,
    Guid? PayerId = null,
    DateTime? ServiceFrom = null,
    DateTime? ServiceTo = null,
    int Page = 1,
    int PageSize = 25);

public sealed record ClaimLineInput(
    int LineNumber,
    string ProcedureCode,
    decimal Units,
    decimal ChargeAmount,
    DateTime ServiceDate,
    string? Modifier1 = null,
    string? Modifier2 = null,
    string? Modifier3 = null,
    string? Modifier4 = null,
    string? PlaceOfService = null,
    string? DiagnosisPointers = null);

public sealed record ClaimLineDto(
    Guid Id,
    int LineNumber,
    string ProcedureCode,
    string? Modifier1,
    string? Modifier2,
    string? Modifier3,
    string? Modifier4,
    string? PlaceOfService,
    decimal Units,
    decimal ChargeAmount,
    DateTime ServiceDate,
    string? DiagnosisPointers);

public sealed record ClaimDto(
    Guid Id,
    long TenantId,
    Guid PatientId,
    Guid PatientInsuranceId,
    Guid PayerId,
    Guid ProviderId,
    string ClaimNumber,
    string ClaimType,
    string Status,
    DateTime ServiceFrom,
    DateTime ServiceTo,
    decimal TotalChargeAmount,
    string? PayerClaimNumber,
    string? SubmissionReference,
    DateTime? SubmittedAtUtc,
    DateTime? LastStatusCheckedAtUtc,
    string? Notes,
    IReadOnlyList<ClaimLineDto> Lines,
    DateTime CreatedAtUtc,
    DateTime? UpdatedAtUtc);

public sealed record CreateClaimRequest(
    Guid PatientId,
    Guid PatientInsuranceId,
    Guid PayerId,
    Guid ProviderId,
    string ClaimNumber,
    string ClaimType,
    DateTime ServiceFrom,
    DateTime ServiceTo,
    string? Notes = null,
    IReadOnlyList<ClaimLineInput>? Lines = null);

public sealed record UpdateClaimRequest(
    Guid PatientId,
    Guid PatientInsuranceId,
    Guid PayerId,
    Guid ProviderId,
    string ClaimType,
    DateTime ServiceFrom,
    DateTime ServiceTo,
    string? Notes = null,
    IReadOnlyList<ClaimLineInput>? Lines = null);

public sealed record ClaimValidationDto(
    Guid ClaimId,
    bool IsValid,
    IReadOnlyList<string> Errors,
    IReadOnlyList<string> Warnings,
    decimal CalculatedTotalChargeAmount,
    DateTime ValidatedAtUtc);
