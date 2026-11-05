namespace InsureFlow.Contracts.Authorizations;

// ============================================================
// Authorization list, details, create, and update
// ============================================================

public sealed record AuthorizationListRequest(
    string? Search = null,
    string? Status = null,
    string? Priority = null,
    int Page = 1,
    int PageSize = 25);

public sealed record AuthorizationDto(
    Guid Id,
    long TenantId, 
    Guid PatientId,
    Guid PatientInsuranceId,
    Guid PayerId,
    Guid ProviderId,
    string AuthorizationNumber,
    string RequestType,
    string Status,
    string Priority,
    string? PlaceOfService,
    DateTime? RequestedFrom,
    DateTime? RequestedTo,
    DateTime? DecisionDate,
    string? PayerReference,
    string? ClinicalSummary,
    string? DecisionReason,
    DateTime CreatedAtUtc,
    DateTime? UpdatedAtUtc);

public sealed record CreateAuthorizationRequest(
    Guid PatientId,
    Guid PatientInsuranceId,
    Guid PayerId,
    Guid ProviderId,
    string AuthorizationNumber,
    DateTime? RequestedFrom = null,
    DateTime? RequestedTo = null,
    string? RequestType = null,
    string? Priority = null,
    string? PlaceOfService = null,
    string? ClinicalSummary = null);

public sealed record UpdateAuthorizationRequest(
    string? RequestType = null,
    string? Priority = null,
    string? PlaceOfService = null,
    DateTime? RequestedFrom = null,
    DateTime? RequestedTo = null,
    string? ClinicalSummary = null);

// ============================================================
// Authorization service lines
// ============================================================

public sealed record AuthorizationServiceLineDto(
    long Id,
    string CptHcpcsCode,
    string? Modifier,
    string? DiagnosisCode,
    int UnitsRequested,
    int? UnitsApproved,
    decimal? RequestedAmount,
    decimal? ApprovedAmount,
    string Status);

public sealed record AddAuthorizationServiceLineRequest(
    string CptHcpcsCode,
    int UnitsRequested,
    decimal? RequestedAmount = null,
    string? Modifier = null,
    string? DiagnosisCode = null);

public sealed record UpdateAuthorizationServiceLineRequest(
    int? UnitsRequested = null,
    int? UnitsApproved = null,
    decimal? RequestedAmount = null,
    decimal? ApprovedAmount = null,
    string? Status = null);

// ============================================================
// Authorization diagnoses
// ============================================================

public sealed record AuthorizationDiagnosisDto(
    long Id,
    string Icd10Code,
    string? Description,
    int Sequence);

public sealed record AddAuthorizationDiagnosisRequest(
    string Icd10Code,
    string? Description = null,
    int Sequence = 1);

// ============================================================
// Authorization notes
// ============================================================

public sealed record AuthorizationNoteDto(
    long Id,
    string NoteType,
    string NoteText,
    DateTime CreatedAt);

public sealed record AddAuthorizationNoteRequest(
    string NoteType,
    string NoteText);

// ============================================================
// Authorization history
// ============================================================

public sealed record AuthorizationHistoryDto(
    long Id,
    string FromStatus,
    string ToStatus,
    string? Note,
    DateTime ChangedAt,
    long? ChangedByUserId);

// ============================================================
// Authorization documents
// ============================================================

public sealed record AuthorizationDocumentDto(
    long Id,
    string FileName,
    string StorageKey,
    string? ContentType,
    long? FileSize,
    string DocumentType,
    DateTime UploadedAt);

public sealed record AddAuthorizationDocumentRequest(
    string FileName,
    string StorageKey,
    string? ContentType = null,
    long? FileSize = null,
    string? DocumentType = null);

// ============================================================
// Authorization appeals
// ============================================================

public sealed record AuthorizationAppealDto(
    long Id,
    string Reason,
    string? Notes,
    string Status,
    DateTime SubmittedAt);

public sealed record CreateAuthorizationAppealRequest(
    string Reason,
    string? Notes = null);

// ============================================================
// Authorization workflow actions
// ============================================================

public sealed record AuthorizationActionRequest(
    string? Note = null);

public sealed record DenyAuthorizationRequest(
    string Reason);
