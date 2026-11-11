namespace InsureFlow.Contracts.Denials;

public sealed class DenialAttachmentResponse
{
    public Guid ExternalId { get; set; }

    public string FileName { get; set; } = string.Empty;

    public string ContentType { get; set; } = string.Empty;

    public long FileSizeBytes { get; set; }

    public string StorageReference { get; set; } = string.Empty;

    public long? UploadedByUserId { get; set; }

    public DateTime UploadedAtUtc { get; set; }

    public DateTime CreatedAtUtc { get; set; }
}