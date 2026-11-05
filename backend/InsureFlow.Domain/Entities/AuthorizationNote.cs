namespace InsureFlow.Domain.Entities;

public sealed class AuthorizationNote
{
    // Required by Entity Framework Core.
    private AuthorizationNote()
    {
        NoteType = string.Empty;
        NoteText = string.Empty;
    }

    public AuthorizationNote(
        long tenantId,
        long authorizationId,
        string noteType,
        string noteText)
    {
        if (tenantId <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(tenantId),
                "Tenant ID must be greater than zero.");
        }

        if (authorizationId <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(authorizationId),
                "Authorization ID must be greater than zero.");
        }

        if (string.IsNullOrWhiteSpace(noteType))
        {
            throw new ArgumentException(
                "Note type is required.",
                nameof(noteType));
        }

        if (string.IsNullOrWhiteSpace(noteText))
        {
            throw new ArgumentException(
                "Note text is required.",
                nameof(noteText));
        }

        TenantId = tenantId;
        AuthorizationId = authorizationId;
        NoteType = noteType.Trim().ToUpperInvariant();
        NoteText = noteText.Trim();
        CreatedAt = DateTime.UtcNow;
    }

    public long Id { get; private set; }

    public long TenantId { get; private set; }

    public long AuthorizationId { get; private set; }

    public string NoteType { get; private set; }

    public string NoteText { get; private set; }

    public DateTime CreatedAt { get; private set; }
}
