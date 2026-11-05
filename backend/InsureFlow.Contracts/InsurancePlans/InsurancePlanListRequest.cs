namespace InsureFlow.Contracts.InsurancePlans;

public sealed record InsurancePlanListRequest(
    string? Search = null,
    string? Status = null,
    string? PlanType = null,
    string? NetworkType = null,
    Guid? PayerId = null,
    int Page = 1,
    int PageSize = 25);