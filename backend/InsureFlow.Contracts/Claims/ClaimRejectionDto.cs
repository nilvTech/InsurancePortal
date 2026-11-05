
namespace InsureFlow.Contracts.Claims;

/// <summary>
/// Represents a rejection reported for a healthcare insurance claim.
/// </summary>
public sealed class ClaimRejectionDto
{
    // The existing database rejection ID is a long, not a GUID.
    public long Id { get; set; }

    // Public claim identifier.
    public Guid ClaimId { get; set; }

    public string RejectionCode { get; set; } = string.Empty;

    public string RejectionMessage { get; set; } = string.Empty;

    public string? Source { get; set; }

    public string? ExternalReference { get; set; }

    public DateTime RejectedAt { get; set; }
}
