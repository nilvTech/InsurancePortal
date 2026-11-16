
using InsureFlow.Application.Persistence;
using InsureFlow.Application.Tenants;
using InsureFlow.Contracts.Appeals;
using InsureFlow.Domain.Entities;

namespace InsureFlow.Application.Appeals;

public sealed class AppealService : IAppealService
{
    private readonly IAppealRepository _appealRepository;
    private readonly ITenantContext _tenantContext;

    public AppealService(
        IAppealRepository appealRepository,
        ITenantContext tenantContext)
    {
        _appealRepository = appealRepository;
        _tenantContext = tenantContext;
    }

    // Main appeals

    public async Task<IReadOnlyList<AppealResponse>> GetPagedAsync(
        string? search,
        string? status,
        Guid? denialExternalId,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        RequireTenant();
        ValidatePaging(page, pageSize);

        var appeals = await _appealRepository.GetPagedAsync(
            search,
            status,
            denialExternalId,
            page,
            pageSize,
            cancellationToken);

        var responses = new List<AppealResponse>(appeals.Count);

        foreach (var appeal in appeals)
        {
            responses.Add(await MapAsync(appeal, cancellationToken));
        }

        return responses;
    }

    public async Task<AppealResponse?> GetByExternalIdAsync(
        Guid externalId,
        CancellationToken cancellationToken = default)
    {
        RequireTenant();

        var appeal = await _appealRepository.GetByExternalIdAsync(
            externalId,
            cancellationToken);

        return appeal is null
            ? null
            : await MapAsync(appeal, cancellationToken);
    }

    public async Task<AppealResponse> CreateAsync(
        CreateAppealRequest request,
        CancellationToken cancellationToken = default)
    {
        var tenantId = RequireTenant();

        ArgumentNullException.ThrowIfNull(request);

        var denial = await _appealRepository.GetDenialByExternalIdAsync(
            request.DenialExternalId,
            cancellationToken);

        if (denial is null || denial.TenantId != tenantId)
        {
            throw new ArgumentException(
                "The specified denial was not found.");
        }

        if (string.IsNullOrWhiteSpace(request.AppealNumber))
            throw new ArgumentException("Appeal number is required.");

        if (await _appealRepository.ExistsAppealNumberAsync(
                request.AppealNumber.Trim(),
                cancellationToken))
        {
            throw new InvalidOperationException(
                "An appeal with this appeal number already exists.");
        }

        var appeal = new Appeal(
            tenantId,
            denial.Id,
            request.AppealNumber,
            request.AppealLevel,
            request.AppealReason,
            request.AppealNotes,
            request.AppealDeadlineUtc);

        await _appealRepository.AddAsync(
            appeal,
            cancellationToken);

        await AddHistoryAsync(
            appeal,
            "CREATED",
            "Appeal was created.",
            cancellationToken);

        await _appealRepository.SaveChangesAsync(cancellationToken);

        return await MapAsync(appeal, cancellationToken);
    }

    public async Task<AppealResponse?> UpdateAsync(
        Guid externalId,
        UpdateAppealRequest request,
        CancellationToken cancellationToken = default)
    {
        RequireTenant();
        ArgumentNullException.ThrowIfNull(request);

        var appeal = await GetAppealAsync(
            externalId,
            cancellationToken);

        if (appeal is null)
            return null;

        appeal.UpdateDetails(
            request.AppealLevel,
            request.AppealReason,
            request.AppealNotes,
            request.AppealDeadlineUtc);

        await AddHistoryAsync(
            appeal,
            "UPDATED",
            "Appeal details were updated.",
            cancellationToken);

        await _appealRepository.SaveChangesAsync(cancellationToken);

        return await MapAsync(appeal, cancellationToken);
    }

    public async Task<AppealResponse?> SubmitAsync(
        Guid externalId,
        CancellationToken cancellationToken = default)
    {
        var tenantId = RequireTenant();

        var appeal = await GetAppealAsync(
            externalId,
            cancellationToken);

        if (appeal is null)
            return null;

        var denial = await _appealRepository.GetDenialByIdAsync(
            appeal.DenialId,
            cancellationToken);

        if (denial is null || denial.TenantId != tenantId)
        {
            throw new InvalidOperationException(
                "The denial linked to this appeal could not be found.");
        }

        if (denial.Status == "OPEN")
        {
            denial.MarkAppealed();
        }
        else if (denial.Status != "APPEALED")
        {
            throw new InvalidOperationException(
                "Only open or already appealed denials can be submitted.");
        }

        appeal.Submit();

        await AddHistoryAsync(
            appeal,
            "SUBMITTED",
            "Appeal was submitted.",
            cancellationToken);

        await AddNotificationAsync(
            appeal,
            "APPEAL_SUBMITTED",
            "Appeal submitted",
            $"Appeal {appeal.AppealNumber} was submitted.",
            cancellationToken);

        await _appealRepository.SaveChangesAsync(cancellationToken);

        return await MapAsync(appeal, cancellationToken);
    }

    public async Task<AppealResponse?> WithdrawAsync(
        Guid externalId,
        CancellationToken cancellationToken = default)
    {
        var appeal = await GetAppealAsync(
            externalId,
            cancellationToken);

        if (appeal is null)
            return null;

        appeal.Withdraw();

        await AddHistoryAsync(
            appeal,
            "WITHDRAWN",
            "Appeal was withdrawn.",
            cancellationToken);

        await AddNotificationAsync(
            appeal,
            "APPEAL_WITHDRAWN",
            "Appeal withdrawn",
            $"Appeal {appeal.AppealNumber} was withdrawn.",
            cancellationToken);

        await _appealRepository.SaveChangesAsync(cancellationToken);

        return await MapAsync(appeal, cancellationToken);
    }

    public async Task<AppealResponse?> RecordDecisionAsync(
        Guid externalId,
        RecordAppealDecisionRequest request,
        CancellationToken cancellationToken = default)
    {
        RequireTenant();
        ArgumentNullException.ThrowIfNull(request);

        var appeal = await GetAppealAsync(
            externalId,
            cancellationToken);

        if (appeal is null)
            return null;

        appeal.RecordDecision(
            request.Decision,
            request.DecisionNotes);

        await AddHistoryAsync(
            appeal,
            "DECISION_RECORDED",
            $"Appeal decision recorded: {appeal.Decision}.",
            cancellationToken);

        await AddNotificationAsync(
            appeal,
            "APPEAL_DECISION",
            "Appeal decision recorded",
            $"A decision was recorded for appeal {appeal.AppealNumber}.",
            cancellationToken);

        await _appealRepository.SaveChangesAsync(cancellationToken);

        return await MapAsync(appeal, cancellationToken);
    }

    // Documentation

    public async Task<IReadOnlyList<AppealDocumentationResponse>>
        GetDocumentationAsync(
            Guid appealExternalId,
            CancellationToken cancellationToken = default)
    {
        RequireTenant();

        var appeal = await GetAppealAsync(
            appealExternalId,
            cancellationToken);

        if (appeal is null)
            return Array.Empty<AppealDocumentationResponse>();

        var documents = await _appealRepository.GetDocumentationAsync(
            appeal.Id,
            cancellationToken);

        return documents.Select(x => new AppealDocumentationResponse(
            x.ExternalId,
            appeal.ExternalId,
            x.DocumentTitle,
            x.DocumentType,
            x.Description,
            x.StorageReference,
            x.CreatedAtUtc)).ToList();
    }

    public async Task<AppealDocumentationResponse?> AddDocumentationAsync(
        Guid appealExternalId,
        AppealDocumentationRequest request,
        CancellationToken cancellationToken = default)
    {
        var tenantId = RequireTenant();
        ArgumentNullException.ThrowIfNull(request);

        var appeal = await GetAppealAsync(
            appealExternalId,
            cancellationToken);

        if (appeal is null)
            return null;

        var documentation = new AppealDocumentation(
            tenantId,
            appeal.Id,
            request.DocumentTitle,
            request.DocumentType,
            request.Description,
            request.StorageReference);

        await _appealRepository.AddDocumentationAsync(
            documentation,
            cancellationToken);

        await AddHistoryAsync(
            appeal,
            "DOCUMENTATION_ADDED",
            $"Documentation '{documentation.DocumentTitle}' was added.",
            cancellationToken);

        await _appealRepository.SaveChangesAsync(cancellationToken);

        return new AppealDocumentationResponse(
            documentation.ExternalId,
            appeal.ExternalId,
            documentation.DocumentTitle,
            documentation.DocumentType,
            documentation.Description,
            documentation.StorageReference,
            documentation.CreatedAtUtc);
    }

    // Attachments

    public async Task<IReadOnlyList<AppealAttachmentResponse>>
        GetAttachmentsAsync(
            Guid appealExternalId,
            CancellationToken cancellationToken = default)
    {
        RequireTenant();

        var appeal = await GetAppealAsync(
            appealExternalId,
            cancellationToken);

        if (appeal is null)
            return Array.Empty<AppealAttachmentResponse>();

        var attachments = await _appealRepository.GetAttachmentsAsync(
            appeal.Id,
            cancellationToken);

        return attachments
            .Select(x => MapAttachment(x, appeal.ExternalId))
            .ToList();
    }

    public async Task<AppealAttachmentResponse?> AddAttachmentAsync(
        Guid appealExternalId,
        AppealAttachmentRequest request,
        CancellationToken cancellationToken = default)
    {
        var tenantId = RequireTenant();
        ArgumentNullException.ThrowIfNull(request);

        var appeal = await GetAppealAsync(
            appealExternalId,
            cancellationToken);

        if (appeal is null)
            return null;

        var attachment = new AppealAttachment(
            tenantId,
            appeal.Id,
            request.FileName,
            request.ContentType,
            request.FileSizeBytes,
            request.StorageReference);

        await _appealRepository.AddAttachmentAsync(
            attachment,
            cancellationToken);

        await AddHistoryAsync(
            appeal,
            "ATTACHMENT_ADDED",
            $"Attachment '{attachment.FileName}' was added.",
            cancellationToken);

        await _appealRepository.SaveChangesAsync(cancellationToken);

        return MapAttachment(attachment, appeal.ExternalId);
    }

    public async Task<bool> DeleteAttachmentAsync(
        Guid appealExternalId,
        Guid attachmentExternalId,
        CancellationToken cancellationToken = default)
    {
        RequireTenant();

        var appeal = await GetAppealAsync(
            appealExternalId,
            cancellationToken);

        if (appeal is null)
            return false;

        var attachment =
            await _appealRepository.GetAttachmentByExternalIdAsync(
                appeal.Id,
                attachmentExternalId,
                cancellationToken);

        if (attachment is null)
            return false;

        _appealRepository.RemoveAttachment(attachment);

        await AddHistoryAsync(
            appeal,
            "ATTACHMENT_DELETED",
            $"Attachment '{attachment.FileName}' was deleted.",
            cancellationToken);

        await _appealRepository.SaveChangesAsync(cancellationToken);

        return true;
    }

    // External reviews

    public async Task<AppealExternalReviewResponse?>
        CreateExternalReviewAsync(
            Guid appealExternalId,
            AppealExternalReviewRequest request,
            CancellationToken cancellationToken = default)
    {
        var tenantId = RequireTenant();
        ArgumentNullException.ThrowIfNull(request);

        var appeal = await GetAppealAsync(
            appealExternalId,
            cancellationToken);

        if (appeal is null)
            return null;

        var externalReview = new AppealExternalReview(
            tenantId,
            appeal.Id,
            request.ReviewingOrganization,
            request.ExternalReferenceNumber,
            request.ReviewDueAtUtc,
            request.ReviewNotes);

        await _appealRepository.AddExternalReviewAsync(
            externalReview,
            cancellationToken);

        await AddHistoryAsync(
            appeal,
            "EXTERNAL_REVIEW_REQUESTED",
            $"External review was recorded for {externalReview.ReviewingOrganization}.",
            cancellationToken);

        await AddNotificationAsync(
            appeal,
            "EXTERNAL_REVIEW_REQUESTED",
            "External review recorded",
            $"An external review was recorded for appeal {appeal.AppealNumber}.",
            cancellationToken);

        await _appealRepository.SaveChangesAsync(cancellationToken);

        return MapExternalReview(
            externalReview,
            appeal.ExternalId);
    }

    public async Task<IReadOnlyList<AppealExternalReviewResponse>>
        GetExternalReviewsAsync(
            string? status,
            int page,
            int pageSize,
            CancellationToken cancellationToken = default)
    {
        RequireTenant();
        ValidatePaging(page, pageSize);

        var reviews = await _appealRepository.GetExternalReviewsAsync(
            null,
            status,
            page,
            pageSize,
            cancellationToken);

        var responses =
            new List<AppealExternalReviewResponse>(reviews.Count);

        foreach (var review in reviews)
        {
            var appealExternalId =
                await _appealRepository.GetAppealExternalIdAsync(
                    review.AppealId,
                    cancellationToken);

            if (appealExternalId.HasValue)
            {
                responses.Add(MapExternalReview(
                    review,
                    appealExternalId.Value));
            }
        }

        return responses;
    }

    // History

    public async Task<IReadOnlyList<AppealHistoryResponse>> GetHistoryAsync(
        Guid appealExternalId,
        CancellationToken cancellationToken = default)
    {
        RequireTenant();

        var appeal = await GetAppealAsync(
            appealExternalId,
            cancellationToken);

        if (appeal is null)
            return Array.Empty<AppealHistoryResponse>();

        var history = await _appealRepository.GetHistoryAsync(
            appeal.Id,
            cancellationToken);

        return history.Select(x => new AppealHistoryResponse(
            x.ExternalId,
            appeal.ExternalId,
            x.ActivityType,
            x.Description,
            x.PerformedByUserId,
            x.ActivityDateUtc)).ToList();
    }

    // Notifications

    public async Task<IReadOnlyList<AppealNotificationResponse>>
        GetNotificationsAsync(
            Guid appealExternalId,
            CancellationToken cancellationToken = default)
    {
        RequireTenant();

        var appeal = await GetAppealAsync(
            appealExternalId,
            cancellationToken);

        if (appeal is null)
            return Array.Empty<AppealNotificationResponse>();

        var notifications = await _appealRepository.GetNotificationsAsync(
            appeal.Id,
            cancellationToken);

        return notifications.Select(x => new AppealNotificationResponse(
            x.ExternalId,
            appeal.ExternalId,
            x.NotificationType,
            x.Title,
            x.Message,
            x.RecipientUserId,
            x.IsRead,
            x.ReadAtUtc,
            x.CreatedAtUtc)).ToList();
    }

    public async Task<int> MarkNotificationsReadAsync(
        Guid appealExternalId,
        MarkAppealNotificationsReadRequest request,
        CancellationToken cancellationToken = default)
    {
        RequireTenant();
        ArgumentNullException.ThrowIfNull(request);

        var appeal = await GetAppealAsync(
            appealExternalId,
            cancellationToken);

        if (appeal is null)
            return -1;

        var notifications =
            await _appealRepository.GetNotificationsAsync(
                appeal.Id,
                cancellationToken);

        var selectedIds = request.NotificationExternalIds?
            .ToHashSet();

        var unreadNotifications = notifications
            .Where(x => !x.IsRead)
            .Where(x =>
                selectedIds is null ||
                selectedIds.Count == 0 ||
                selectedIds.Contains(x.ExternalId))
            .ToList();

        foreach (var notification in unreadNotifications)
        {
            notification.MarkAsRead();
        }

        if (unreadNotifications.Count > 0)
        {
            await AddHistoryAsync(
                appeal,
                "NOTIFICATIONS_READ",
                $"{unreadNotifications.Count} appeal notification(s) marked as read.",
                cancellationToken);

            await _appealRepository.SaveChangesAsync(cancellationToken);
        }

        return unreadNotifications.Count;
    }

    // Private helpers

    private long RequireTenant()
    {
        return _tenantContext.TenantId
            ?? throw new InvalidOperationException(
                "A tenant context is required for this operation.");
    }

    private Task<Appeal?> GetAppealAsync(
        Guid externalId,
        CancellationToken cancellationToken)
    {
        return _appealRepository.GetByExternalIdAsync(
            externalId,
            cancellationToken);
    }

    private async Task<AppealResponse> MapAsync(
        Appeal appeal,
        CancellationToken cancellationToken)
    {
        var denialExternalId =
            await _appealRepository.GetDenialExternalIdAsync(
                appeal.DenialId,
                cancellationToken);

        return new AppealResponse(
            appeal.ExternalId,
            denialExternalId ?? Guid.Empty,
            appeal.AppealNumber,
            appeal.AppealLevel,
            appeal.AppealReason,
            appeal.AppealNotes,
            appeal.Status,
            appeal.AppealDeadlineUtc,
            appeal.SubmittedAtUtc,
            appeal.WithdrawnAtUtc,
            appeal.Decision,
            appeal.DecisionNotes,
            appeal.DecisionAtUtc,
            appeal.CreatedAtUtc,
            appeal.UpdatedAtUtc);
    }

    private async Task AddHistoryAsync(
        Appeal appeal,
        string activityType,
        string description,
        CancellationToken cancellationToken)
    {
        var history = new AppealHistory(
            RequireTenant(),
            appeal.Id,
            activityType,
            description);

        await _appealRepository.AddHistoryAsync(
            history,
            cancellationToken);
    }

    private async Task AddNotificationAsync(
        Appeal appeal,
        string notificationType,
        string title,
        string message,
        CancellationToken cancellationToken)
    {
        var notification = new AppealNotification(
            RequireTenant(),
            appeal.Id,
            notificationType,
            title,
            message);

        await _appealRepository.AddNotificationAsync(
            notification,
            cancellationToken);
    }

    private static AppealAttachmentResponse MapAttachment(
        AppealAttachment attachment,
        Guid appealExternalId)
    {
        return new AppealAttachmentResponse(
            attachment.ExternalId,
            appealExternalId,
            attachment.FileName,
            attachment.ContentType,
            attachment.FileSizeBytes,
            attachment.StorageReference,
            attachment.UploadedAtUtc,
            attachment.UploadedByUserId);
    }

    private static AppealExternalReviewResponse MapExternalReview(
        AppealExternalReview review,
        Guid appealExternalId)
    {
        return new AppealExternalReviewResponse(
            review.ExternalId,
            appealExternalId,
            review.ReviewingOrganization,
            review.ExternalReferenceNumber,
            review.Status,
            review.RequestedAtUtc,
            review.ReviewDueAtUtc,
            review.RespondedAtUtc,
            review.ReviewNotes);
    }

    private static void ValidatePaging(int page, int pageSize)
    {
        if (page < 1)
            throw new ArgumentOutOfRangeException(
                nameof(page),
                "Page must be greater than zero.");

        if (pageSize < 1 || pageSize > 100)
            throw new ArgumentOutOfRangeException(
                nameof(pageSize),
                "Page size must be between 1 and 100.");
    }
}
