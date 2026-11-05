using InsureFlow.Application.Abstractions;
using InsureFlow.Application.Persistence;
using InsureFlow.Application.Tenants;
using InsureFlow.Contracts.InsurancePlans;
using InsureFlow.Domain.Entities;
using InsureFlow.Domain.Enums;

namespace InsureFlow.Application.InsurancePlans;

public sealed class InsurancePlanService
    : IInsurancePlanService
{
    private readonly IInsurancePlanRepository _repository;
    private readonly IPayerRepository _payerRepository;
    private readonly ITenantContext _tenantContext;
    private readonly IAuditService _auditService;

    public InsurancePlanService(
        IInsurancePlanRepository repository,
        IPayerRepository payerRepository,
        ITenantContext tenantContext,
        IAuditService auditService)
    {
        _repository = repository;
        _payerRepository = payerRepository;
        _tenantContext = tenantContext;
        _auditService = auditService;
    }

    public async Task<IReadOnlyList<InsurancePlanDto>> GetPagedAsync(
        InsurancePlanListRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var page = request.Page < 1
            ? 1
            : request.Page;

        var pageSize = request.PageSize switch
        {
            < 1 => 25,
            > 100 => 100,
            _ => request.PageSize
        };

        long? payerId = null;

        if (request.PayerId.HasValue)
        {
            var payer =
                await _payerRepository.GetByExternalIdAsync(
                    request.PayerId.Value,
                    cancellationToken);

            if (payer is null)
            {
                return [];
            }

            payerId = payer.Id;
        }

        var plans =
            await _repository.GetPagedAsync(
                request.Search,
                request.Status,
                request.PlanType,
                request.NetworkType,
                payerId,
                page,
                pageSize,
                cancellationToken);

        return plans
            .Select(MapToDto)
            .ToList();
    }

    public async Task<InsurancePlanDto?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException(
                "Insurance plan ID is required.",
                nameof(id));
        }

        var plan =
            await _repository.GetByExternalIdAsync(
                id,
                cancellationToken);

        return plan is null
            ? null
            : MapToDto(plan);
    }

    public async Task<InsurancePlanDto> CreateAsync(
        CreateInsurancePlanRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var tenantId = RequireTenant();

        var planCode = NormalizeRequired(
            request.PlanCode,
            nameof(request.PlanCode),
            "Plan code is required.")
            .ToUpperInvariant();

        var planName = NormalizeRequired(
            request.PlanName,
            nameof(request.PlanName),
            "Plan name is required.");

        if (request.PayerId == Guid.Empty)
        {
            throw new ArgumentException(
                "Payer ID is required.",
                nameof(request.PayerId));
        }

        var payer =
            await _payerRepository.GetByExternalIdAsync(
                request.PayerId,
                cancellationToken);

        if (payer is null)
        {
            throw new InvalidOperationException(
                "The specified payer was not found for the current tenant.");
        }

        var planType = ParseEnum<InsurancePlanType>(
            request.PlanType,
            nameof(request.PlanType));

        var networkType =
            ParseNullableEnum<InsuranceNetworkType>(
                request.NetworkType,
                nameof(request.NetworkType));

        if (await _repository.ExistsByCodeAsync(
                planCode,
                cancellationToken: cancellationToken))
        {
            throw new InvalidOperationException(
                $"An insurance plan with code '{planCode}' already exists.");
        }

        var insurancePlan = new InsurancePlan(
            tenantId,
            payer.Id,
            planCode,
            planName,
            planType,
            networkType);

        await _repository.AddAsync(
            insurancePlan,
            cancellationToken);

        await _repository.SaveChangesAsync(
            cancellationToken);

        await _auditService.WriteAsync(
            new AuditEntry(
                TenantId: tenantId,
                UserId: null,
                Action: "InsurancePlanCreated",
                EntityName: nameof(InsurancePlan),
                EntityId: insurancePlan.ExternalId.ToString()),
            cancellationToken);

        return MapToDto(insurancePlan);
    }

    public async Task<bool> UpdateAsync(
        Guid id,
        UpdateInsurancePlanRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var tenantId = RequireTenant();

        var plan =
            await _repository.GetByExternalIdAsync(
                id,
                cancellationToken);

        if (plan is null)
        {
            return false;
        }

        if (request.PayerId == Guid.Empty)
        {
            throw new ArgumentException(
                "Payer ID is required.",
                nameof(request.PayerId));
        }

        var payer =
            await _payerRepository.GetByExternalIdAsync(
                request.PayerId,
                cancellationToken);

        if (payer is null)
        {
            throw new InvalidOperationException(
                "The specified payer was not found for the current tenant.");
        }

        var planCode = NormalizeRequired(
            request.PlanCode,
            nameof(request.PlanCode),
            "Plan code is required.")
            .ToUpperInvariant();

        var planName = NormalizeRequired(
            request.PlanName,
            nameof(request.PlanName),
            "Plan name is required.");

        if (await _repository.ExistsByCodeAsync(
                planCode,
                id,
                cancellationToken))
        {
            throw new InvalidOperationException(
                $"An insurance plan with code '{planCode}' already exists.");
        }

        var planType = ParseEnum<InsurancePlanType>(
            request.PlanType,
            nameof(request.PlanType));

        var networkType =
            ParseNullableEnum<InsuranceNetworkType>(
                request.NetworkType,
                nameof(request.NetworkType));

        plan.UpdateDetails(
            payer.Id,
            planCode,
            planName,
            planType,
            networkType);

        await _repository.UpdateAsync(
            plan,
            cancellationToken);

        await _repository.SaveChangesAsync(
            cancellationToken);

        await _auditService.WriteAsync(
            new AuditEntry(
                TenantId: tenantId,
                UserId: null,
                Action: "InsurancePlanUpdated",
                EntityName: nameof(InsurancePlan),
                EntityId: plan.ExternalId.ToString()),
            cancellationToken);

        return true;
    }

    public Task<bool> DeactivateAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return SetStatusAsync(
            id,
            active: false,
            cancellationToken);
    }

    public Task<bool> ActivateAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return SetStatusAsync(
            id,
            active: true,
            cancellationToken);
    }

    private async Task<bool> SetStatusAsync(
        Guid id,
        bool active,
        CancellationToken cancellationToken)
    {
        var tenantId = RequireTenant();

        var plan =
            await _repository.GetByExternalIdAsync(
                id,
                cancellationToken);

        if (plan is null)
        {
            return false;
        }

        if (active)
        {
            plan.Activate();
        }
        else
        {
            plan.Deactivate();
        }

        await _repository.UpdateAsync(
            plan,
            cancellationToken);

        await _repository.SaveChangesAsync(
            cancellationToken);

        await _auditService.WriteAsync(
            new AuditEntry(
                TenantId: tenantId,
                UserId: null,
                Action: active
                    ? "InsurancePlanActivated"
                    : "InsurancePlanDeactivated",
                EntityName: nameof(InsurancePlan),
                EntityId: plan.ExternalId.ToString()),
            cancellationToken);

        return true;
    }

    private long RequireTenant()
    {
        if (!_tenantContext.TenantId.HasValue)
        {
            throw new InvalidOperationException(
                "A tenant context is required for this operation.");
        }

        return _tenantContext.TenantId.Value;
    }

    private static string NormalizeRequired(
        string? value,
        string parameterName,
        string errorMessage)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException(
                errorMessage,
                parameterName);
        }

        return value.Trim();
    }

    private static T ParseEnum<T>(
        string? value,
        string parameterName)
        where T : struct, Enum
    {
        if (string.IsNullOrWhiteSpace(value) ||
            !Enum.TryParse<T>(
                value,
                true,
                out var result))
        {
            throw new ArgumentException(
                $"Invalid {typeof(T).Name}.",
                parameterName);
        }

        return result;
    }

    private static T? ParseNullableEnum<T>(
        string? value,
        string parameterName)
        where T : struct, Enum
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        return ParseEnum<T>(
            value,
            parameterName);
    }

    private static InsurancePlanDto MapToDto(
        InsurancePlan plan)
    {
        return new InsurancePlanDto(
            plan.ExternalId,
            plan.TenantId,
            plan.Payer.ExternalId,
            plan.Payer.Name,
            plan.PlanCode,
            plan.PlanName,
            plan.PlanType.ToString(),
            plan.NetworkType?.ToString(),
            plan.Status.ToString(),
            plan.CreatedAtUtc,
            plan.UpdatedAtUtc);
    }
}