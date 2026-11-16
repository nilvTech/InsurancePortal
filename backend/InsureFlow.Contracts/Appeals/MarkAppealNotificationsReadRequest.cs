
namespace InsureFlow.Contracts.Appeals;

public sealed record MarkAppealNotificationsReadRequest(
    IReadOnlyCollection<Guid>? NotificationExternalIds);
