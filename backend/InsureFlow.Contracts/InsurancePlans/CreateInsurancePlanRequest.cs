namespace InsureFlow.Contracts.InsurancePlans;

public sealed record CreateInsurancePlanRequest(
    Guid PayerId,
    string PlanCode,
    string PlanName,
    string PlanType,
    string? NetworkType);