namespace InsureFlow.Portal.Models.Authorization;

public class AuthorizationHistoryModel
{
    public Guid Id { get; set; }

    public string Status { get; set; } = string.Empty;

    public string Action { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string PerformedBy { get; set; } = string.Empty;

    public DateTime Date { get; set; }
}