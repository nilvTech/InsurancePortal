namespace InsureFlow.Contracts.InsurancePlans;

public sealed record InsurancePlanDto(
    Guid Id,
    long TenantId,
    Guid PayerId,
    string PayerName,
    string PlanCode,
    string PlanName,
    string PlanType,
    string? NetworkType,
    string Status,
    DateTime CreatedAtUtc,
    DateTime? UpdatedAtUtc);