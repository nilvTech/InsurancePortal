namespace InsureFlow.Portal.Models.Authorization;

public class AuthorizationDocumentModel
{
    public Guid Id { get; set; }

    public string FileName { get; set; } = string.Empty;

    public string DocumentType { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public long FileSize { get; set; }

    public string UploadedBy { get; set; } = string.Empty;

    public DateTime UploadedDate { get; set; }

    public string Status { get; set; } = "Uploaded";
}