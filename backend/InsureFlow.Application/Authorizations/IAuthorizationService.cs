using InsureFlow.Contracts.Authorizations;

namespace InsureFlow.Application.Authorizations;

public interface IAuthorizationService
{
    Task<IReadOnlyList<AuthorizationDto>> GetPagedAsync(
        AuthorizationListRequest request,
        CancellationToken ct = default);

    Task<AuthorizationDto?> GetByIdAsync(
        Guid id,
        CancellationToken ct = default);

    Task<AuthorizationDto> CreateAsync(
        CreateAuthorizationRequest request,
        CancellationToken ct = default);

    Task<bool> UpdateAsync(
        Guid id,
        UpdateAuthorizationRequest request,
        CancellationToken ct = default);

    Task<bool> SubmitAsync(
        Guid id,
        string? note,
        CancellationToken ct = default);

    Task<bool> ApproveAsync(
        Guid id,
        string? note,
        CancellationToken ct = default);

    Task<bool> DenyAsync(
        Guid id,
        string reason,
        CancellationToken ct = default);

    Task<bool> CancelAsync(
        Guid id,
        string? reason,
        CancellationToken ct = default);

    Task<IReadOnlyList<AuthorizationServiceLineDto>> GetServicesAsync(
        Guid id,
        CancellationToken ct = default);

    Task<AuthorizationServiceLineDto> AddServiceAsync(
        Guid id,
        AddAuthorizationServiceLineRequest request,
        CancellationToken ct = default);

    Task<bool> UpdateServiceAsync(
        Guid id,
        long serviceId,
        UpdateAuthorizationServiceLineRequest request,
        CancellationToken ct = default);

    Task<bool> DeleteServiceAsync(
        Guid id,
        long serviceId,
        CancellationToken ct = default);

    Task<IReadOnlyList<AuthorizationDiagnosisDto>> GetDiagnosesAsync(
        Guid id,
        CancellationToken ct = default);

    Task<AuthorizationDiagnosisDto> AddDiagnosisAsync(
        Guid id,
        AddAuthorizationDiagnosisRequest request,
        CancellationToken ct = default);

    Task<bool> DeleteDiagnosisAsync(
        Guid id,
        long diagnosisId,
        CancellationToken ct = default);

    Task<IReadOnlyList<AuthorizationDocumentDto>> GetDocumentsAsync(
        Guid id,
        CancellationToken ct = default);

    Task<AuthorizationDocumentDto> AddDocumentAsync(
        Guid id,
        AddAuthorizationDocumentRequest request,
        CancellationToken ct = default);

    Task<bool> DeleteDocumentAsync(
        Guid id,
        long documentId,
        CancellationToken ct = default);

    Task<IReadOnlyList<AuthorizationHistoryDto>> GetHistoryAsync(
        Guid id,
        CancellationToken ct = default);

    Task<IReadOnlyList<AuthorizationNoteDto>> GetNotesAsync(
        Guid id,
        CancellationToken ct = default);

    Task<AuthorizationNoteDto> AddNoteAsync(
        Guid id,
        AddAuthorizationNoteRequest request,
        CancellationToken ct = default);

    Task<AuthorizationAppealDto> CreateAppealAsync(
        Guid id,
        CreateAuthorizationAppealRequest request,
        CancellationToken ct = default);
}