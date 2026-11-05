namespace InsureFlow.Contracts.Organizations;

public sealed record OrganizationListRequest(
    string? Search = null,
    string? Status = null,
    int Page = 1,
    int PageSize = 25);