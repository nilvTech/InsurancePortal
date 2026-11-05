namespace InsureFlow.Contracts.InsurancePlans;

public sealed record UpdateInsurancePlanRequest(
    Guid PayerId,
    string PlanCode,
    string PlanName,
    string PlanType,
    string? NetworkType);