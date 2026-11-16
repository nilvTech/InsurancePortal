
using InsureFlow.Contracts.Appeals;

namespace InsureFlow.Application.Appeals;

public interface IAppealService
{
    // Main appeals

    Task<IReadOnlyList<AppealResponse>> GetPagedAsync(
        string? search,
        string? status,
        Guid? denialExternalId,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default);

    Task<AppealResponse?> GetByExternalIdAsync(
        Guid externalId,
        CancellationToken cancellationToken = default);

    Task<AppealResponse> CreateAsync(
        CreateAppealRequest request,
        CancellationToken cancellationToken = default);

    Task<AppealResponse?> UpdateAsync(
        Guid externalId,
        UpdateAppealRequest request,
        CancellationToken cancellationToken = default);

    Task<AppealResponse?> SubmitAsync(
        Guid externalId,
        CancellationToken cancellationToken = default);

    Task<AppealResponse?> WithdrawAsync(
        Guid externalId,
        CancellationToken cancellationToken = default);

    Task<AppealResponse?> RecordDecisionAsync(
        Guid externalId,
        RecordAppealDecisionRequest request,
        CancellationToken cancellationToken = default);

    // Documentation

    Task<IReadOnlyList<AppealDocumentationResponse>>
        GetDocumentationAsync(
            Guid appealExternalId,
            CancellationToken cancellationToken = default);

    Task<AppealDocumentationResponse?> AddDocumentationAsync(
        Guid appealExternalId,
        AppealDocumentationRequest request,
        CancellationToken cancellationToken = default);

    // Attachments

    Task<IReadOnlyList<AppealAttachmentResponse>> GetAttachmentsAsync(
        Guid appealExternalId,
        CancellationToken cancellationToken = default);

    Task<AppealAttachmentResponse?> AddAttachmentAsync(
        Guid appealExternalId,
        AppealAttachmentRequest request,
        CancellationToken cancellationToken = default);

    Task<bool> DeleteAttachmentAsync(
        Guid appealExternalId,
        Guid attachmentExternalId,
        CancellationToken cancellationToken = default);

    // External reviews

    Task<AppealExternalReviewResponse?> CreateExternalReviewAsync(
        Guid appealExternalId,
        AppealExternalReviewRequest request,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<AppealExternalReviewResponse>>
        GetExternalReviewsAsync(
            string? status,
            int page,
            int pageSize,
            CancellationToken cancellationToken = default);

    // History

    Task<IReadOnlyList<AppealHistoryResponse>> GetHistoryAsync(
        Guid appealExternalId,
        CancellationToken cancellationToken = default);

    // Notifications

    Task<IReadOnlyList<AppealNotificationResponse>>
        GetNotificationsAsync(
            Guid appealExternalId,
            CancellationToken cancellationToken = default);

    Task<int> MarkNotificationsReadAsync(
        Guid appealExternalId,
        MarkAppealNotificationsReadRequest request,
        CancellationToken cancellationToken = default);
}
