using InsureFlow.Domain.Entities;

namespace InsureFlow.Application.Persistence;

public interface IAuthorizationRepository
{
    Task<IReadOnlyList<Authorization>> GetPagedAsync(
        string? search,
        string? status,
        string? priority,
        int page,
        int pageSize,
        CancellationToken ct = default);

    Task<Authorization?> GetByExternalIdAsync(
        Guid externalId,
        CancellationToken ct = default);


    Task<Authorization?> GetByIdAsync(
        long id,
        CancellationToken cancellationToken = default);


    Task<bool> ExistsByNumberAsync(
        string authorizationNumber,
        long? excludeId,
        CancellationToken ct = default);

    Task AddAsync(
        Authorization authorization,
        CancellationToken ct = default);

    Task UpdateAsync(
        Authorization authorization,
        CancellationToken ct = default);

    Task SaveChangesAsync(
        CancellationToken ct = default);

    Task<IReadOnlyList<AuthorizationServiceLine>>
        GetServicesAsync(
            long authorizationId,
            CancellationToken ct = default);

    Task<AuthorizationServiceLine?> GetServiceAsync(
        long serviceId,
        CancellationToken ct = default);

    Task AddServiceAsync(
        AuthorizationServiceLine service,
        CancellationToken ct = default);

    Task<bool> DeleteServiceAsync(
        long authorizationId,
        long serviceId,
        CancellationToken ct = default);

    Task<IReadOnlyList<AuthorizationDiagnosis>>
        GetDiagnosesAsync(
            long authorizationId,
            CancellationToken ct = default);

    Task AddDiagnosisAsync(
        AuthorizationDiagnosis diagnosis,
        CancellationToken ct = default);

    Task<bool> DeleteDiagnosisAsync(
        long authorizationId,
        long diagnosisId,
        CancellationToken ct = default);

    Task<IReadOnlyList<AuthorizationDocument>>
        GetDocumentsAsync(
            long authorizationId,
            CancellationToken ct = default);

    Task AddDocumentAsync(
        AuthorizationDocument document,
        CancellationToken ct = default);

    Task<bool> DeleteDocumentAsync(
        long authorizationId,
        long documentId,
        CancellationToken ct = default);

    Task<IReadOnlyList<AuthorizationHistory>>
        GetHistoryAsync(
            long authorizationId,
            CancellationToken ct = default);

    Task<IReadOnlyList<AuthorizationNote>>
        GetNotesAsync(
            long authorizationId,
            CancellationToken ct = default);

    Task AddNoteAsync(
        AuthorizationNote note,
        CancellationToken ct = default);

    Task AddHistoryAsync(
        AuthorizationHistory history,
        CancellationToken ct = default);

    Task AddAppealAsync(
        AuthorizationAppeal appeal,
        CancellationToken ct = default);
}