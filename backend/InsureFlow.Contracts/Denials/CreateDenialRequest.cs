
namespace InsureFlow.Contracts.Denials;

public sealed record CreateDenialRequest(
    Guid ClaimExternalId,
    string DenialNumber,
    string DenialCode,
    string DenialReason,
    string DenialCategory,
    decimal DeniedAmount,
    DateTime DenialDate,
    DateTime? AppealDeadline);
