using InsureFlow.Domain.Entities;

namespace InsureFlow.Application.Persistence;

public interface IAppealRepository
{
    // Main appeals
    Task<IReadOnlyList<Appeal>> GetPagedAsync(
        string? search,
        string? status,
        Guid? denialExternalId,
        int page,
        int pageSize,
        CancellationToken cancellationToken);

    Task<Appeal?> GetByExternalIdAsync(
        Guid externalId,
        CancellationToken cancellationToken);

    Task<bool> ExistsAppealNumberAsync(
        string appealNumber,
        CancellationToken cancellationToken);

    Task AddAsync(
        Appeal appeal,
        CancellationToken cancellationToken);

    // Denial lookup
    Task<Denial?> GetDenialByExternalIdAsync(
        Guid externalId,
        CancellationToken cancellationToken);

    Task<Denial?> GetDenialByIdAsync(
        long denialId,
        CancellationToken cancellationToken);

    Task<Guid?> GetDenialExternalIdAsync(
        long denialId,
        CancellationToken cancellationToken);

    // Documentation
    Task<IReadOnlyList<AppealDocumentation>> GetDocumentationAsync(
        long appealId,
        CancellationToken cancellationToken);

    Task AddDocumentationAsync(
        AppealDocumentation documentation,
        CancellationToken cancellationToken);

    // Attachments
    Task<IReadOnlyList<AppealAttachment>> GetAttachmentsAsync(
        long appealId,
        CancellationToken cancellationToken);

    Task<AppealAttachment?> GetAttachmentByExternalIdAsync(
        long appealId,
        Guid attachmentExternalId,
        CancellationToken cancellationToken);

    Task AddAttachmentAsync(
        AppealAttachment attachment,
        CancellationToken cancellationToken);

    void RemoveAttachment(AppealAttachment attachment);

    // External reviews
    Task<IReadOnlyList<AppealExternalReview>> GetExternalReviewsAsync(
        long? appealId,
        string? status,
        int page,
        int pageSize,
        CancellationToken cancellationToken);

    Task AddExternalReviewAsync(
        AppealExternalReview externalReview,
        CancellationToken cancellationToken);

    // History
    Task<IReadOnlyList<AppealHistory>> GetHistoryAsync(
        long appealId,
        CancellationToken cancellationToken);

    Task AddHistoryAsync(
        AppealHistory history,
        CancellationToken cancellationToken);

    // Notifications
    Task<IReadOnlyList<AppealNotification>> GetNotificationsAsync(
        long appealId,
        CancellationToken cancellationToken);

    Task SaveChangesAsync(
        CancellationToken cancellationToken);

    Task<Guid?> GetAppealExternalIdAsync(
long appealId,
CancellationToken cancellationToken);

    Task AddNotificationAsync(
        AppealNotification notification,
        CancellationToken cancellationToken);
}