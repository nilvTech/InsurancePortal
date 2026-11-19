namespace InsureFlow.Portal.Models.Authorization;

public class AuthorizationNoteModel
{
    public Guid Id { get; set; }

    public string Note { get; set; } = string.Empty;

    public string CreatedBy { get; set; } = string.Empty;

    public DateTime CreatedDate { get; set; }

    public bool IsInternal { get; set; }
}