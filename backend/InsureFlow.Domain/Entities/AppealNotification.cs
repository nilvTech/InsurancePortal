
using InsureFlow.Domain.Common;

namespace InsureFlow.Domain.Entities;

public sealed class AppealNotification : AuditableEntity
{
    private AppealNotification()
    {
    }

    public AppealNotification(
        long tenantId,
        long appealId,
        string notificationType,
        string title,
        string message,
        long? recipientUserId = null)
    {
        if (tenantId <= 0)
            throw new ArgumentOutOfRangeException(nameof(tenantId));

        if (appealId <= 0)
            throw new ArgumentOutOfRangeException(nameof(appealId));

        if (string.IsNullOrWhiteSpace(notificationType))
            throw new ArgumentException(
                "Notification type is required.",
                nameof(notificationType));

        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException(
                "Notification title is required.",
                nameof(title));

        if (string.IsNullOrWhiteSpace(message))
            throw new ArgumentException(
                "Notification message is required.",
                nameof(message));

        TenantId = tenantId;
        AppealId = appealId;
        NotificationType = notificationType.Trim();
        Title = title.Trim();
        Message = message.Trim();
        RecipientUserId = recipientUserId;
        IsRead = false;
        CreatedAtUtc = DateTime.UtcNow;
    }

    public long TenantId { get; private set; }

    public long AppealId { get; private set; }

    public string NotificationType { get; private set; } = string.Empty;

    public string Title { get; private set; } = string.Empty;

    public string Message { get; private set; } = string.Empty;

    public long? RecipientUserId { get; private set; }

    public bool IsRead { get; private set; }

    public DateTime? ReadAtUtc { get; private set; }

    public void MarkAsRead()
    {
        if (IsRead)
            return;

        IsRead = true;
        ReadAtUtc = DateTime.UtcNow;

        SetUpdatedBy(null);
    }
}
