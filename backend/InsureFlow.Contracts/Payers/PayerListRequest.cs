namespace InsureFlow.Contracts.Payers;

public sealed record PayerListRequest(
    string? Search = null,
    string? Type = null,
    string? Status = null,
    int Page = 1,
    int PageSize = 25);