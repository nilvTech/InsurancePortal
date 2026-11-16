
namespace InsureFlow.Contracts.Appeals;

public sealed record AppealNotificationResponse(
    Guid ExternalId,
    Guid AppealExternalId,
    string NotificationType,
    string Title,
    string Message,
    long? RecipientUserId,
    bool IsRead,
    DateTime? ReadAtUtc,
    DateTime CreatedAtUtc);
