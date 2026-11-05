namespace InsureFlow.Contracts.Providers;

public sealed record ProviderListRequest(
    string? Search = null,
    string? Status = null,
    Guid? OrganizationId = null,
    int Page = 1,
    int PageSize = 25);