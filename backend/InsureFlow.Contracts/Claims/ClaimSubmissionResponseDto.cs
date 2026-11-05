
namespace InsureFlow.Contracts.Claims;

public sealed record ClaimSubmissionResponseDto(
    Guid ClaimId,
    string ClaimStatus,
    string? PayerClaimNumber,
    string? SubmissionReference,
    DateTime? SubmittedAtUtc,
    DateTime? LastStatusCheckedAtUtc);
