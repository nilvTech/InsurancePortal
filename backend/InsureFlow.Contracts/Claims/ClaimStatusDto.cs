namespace InsureFlow.Contracts.Claims;

public sealed record ClaimStatusDto(
    Guid ClaimId,
    string ClaimNumber,
    string Status,
    string? PayerClaimNumber,
    string? SubmissionReference,
    DateTime? SubmittedAtUtc,
    DateTime? LastStatusCheckedAtUtc,
    DateTime? UpdatedAtUtc);