namespace InsureFlow.Contracts.Denials;

public sealed class CreateDenialAttachmentRequest
{
    public string FileName { get; set; } = string.Empty;

    public string ContentType { get; set; } = string.Empty;

    public long FileSizeBytes { get; set; }

    public string StorageReference { get; set; } = string.Empty;
}