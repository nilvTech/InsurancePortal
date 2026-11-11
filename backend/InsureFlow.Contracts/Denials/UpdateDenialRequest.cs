
namespace InsureFlow.Contracts.Denials;

public sealed record UpdateDenialRequest(
    string DenialCode,
    string DenialReason,
    string DenialCategory,
    decimal DeniedAmount,
    DateTime DenialDate,
    DateTime? AppealDeadline);
