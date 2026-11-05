using InsureFlow.Application.Abstractions;
using InsureFlow.Application.Persistence;
using InsureFlow.Application.Tenants;
using InsureFlow.Contracts.Payers;
using InsureFlow.Domain.Entities;
using InsureFlow.Domain.Enums;

namespace InsureFlow.Application.Payers;

public sealed class PayerService : IPayerService
{
    private readonly IPayerRepository _payerRepository;
    private readonly ITenantContext _tenantContext;
    private readonly IAuditService _auditService;

    public PayerService(
        IPayerRepository payerRepository,
        ITenantContext tenantContext,
        IAuditService auditService)
    {
        _payerRepository = payerRepository;
        _tenantContext = tenantContext;
        _auditService = auditService;
    }

    public async Task<IReadOnlyList<PayerDto>> GetPagedAsync(
        PayerListRequest request,
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

        var payers =
            await _payerRepository.GetPagedAsync(
                request.Search,
                request.Type,
                request.Status,
                page,
                pageSize,
                cancellationToken);

        return payers
            .Select(MapToDto)
            .ToList();
    }

    public async Task<PayerDto?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException(
                "Payer ID is required.",
                nameof(id));
        }

        var payer =
            await _payerRepository.GetByExternalIdAsync(
                id,
                cancellationToken);

        return payer is null
            ? null
            : MapToDto(payer);
    }

    public async Task<PayerDto> CreateAsync(
        CreatePayerRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var tenantId = RequireTenant();

        var code = NormalizeRequired(
            request.Code,
            nameof(request.Code),
            "Payer code is required.");

        var name = NormalizeRequired(
            request.Name,
            nameof(request.Name),
            "Payer name is required.");

        if (!Enum.TryParse<PayerType>(
                request.Type,
                true,
                out var payerType))
        {
            throw new ArgumentException(
                "Invalid payer type.",
                nameof(request.Type));
        }

        if (await _payerRepository.ExistsByCodeAsync(
                code,
                cancellationToken: cancellationToken))
        {
            throw new InvalidOperationException(
                $"A payer with code '{code}' already exists.");
        }

        var payer = new Payer(
            tenantId,
            code,
            name,
            payerType);

        payer.UpdateDetails(
            code,
            name,
            request.PayerId,
            payerType,
            request.Phone,
            request.Website);

        await _payerRepository.AddAsync(
            payer,
            cancellationToken);

        await _payerRepository.SaveChangesAsync(
            cancellationToken);

        await _auditService.WriteAsync(
            new AuditEntry(
                TenantId: tenantId,
                UserId: null,
                Action: "PayerCreated",
                EntityName: nameof(Payer),
                EntityId: payer.ExternalId.ToString()),
            cancellationToken);

        return MapToDto(payer);
    }

    public async Task<bool> UpdateAsync(
        Guid id,
        UpdatePayerRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var tenantId = RequireTenant();

        var payer =
            await _payerRepository.GetByExternalIdAsync(
                id,
                cancellationToken);

        if (payer is null)
        {
            return false;
        }

        var code = NormalizeRequired(
            request.Code,
            nameof(request.Code),
            "Payer code is required.");

        var name = NormalizeRequired(
            request.Name,
            nameof(request.Name),
            "Payer name is required.");

        if (!Enum.TryParse<PayerType>(
                request.Type,
                true,
                out var payerType))
        {
            throw new ArgumentException(
                "Invalid payer type.",
                nameof(request.Type));
        }

        if (await _payerRepository.ExistsByCodeAsync(
                code,
                id,
                cancellationToken))
        {
            throw new InvalidOperationException(
                $"A payer with code '{code}' already exists.");
        }

        payer.UpdateDetails(
            code,
            name,
            request.PayerId,
            payerType,
            request.Phone,
            request.Website);

        await _payerRepository.UpdateAsync(
            payer,
            cancellationToken);

        await _payerRepository.SaveChangesAsync(
            cancellationToken);

        await _auditService.WriteAsync(
            new AuditEntry(
                TenantId: tenantId,
                UserId: null,
                Action: "PayerUpdated",
                EntityName: nameof(Payer),
                EntityId: payer.ExternalId.ToString()),
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

        var payer =
            await _payerRepository.GetByExternalIdAsync(
                id,
                cancellationToken);

        if (payer is null)
        {
            return false;
        }

        if (active)
        {
            payer.Activate();
        }
        else
        {
            payer.Deactivate();
        }

        await _payerRepository.UpdateAsync(
            payer,
            cancellationToken);

        await _payerRepository.SaveChangesAsync(
            cancellationToken);

        await _auditService.WriteAsync(
            new AuditEntry(
                TenantId: tenantId,
                UserId: null,
                Action: active
                    ? "PayerActivated"
                    : "PayerDeactivated",
                EntityName: nameof(Payer),
                EntityId: payer.ExternalId.ToString()),
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

    private static PayerDto MapToDto(
        Payer payer)
    {
        return new PayerDto(
            payer.ExternalId,
            payer.TenantId,
            payer.Code,
            payer.Name,
            payer.PayerId,
            payer.Type.ToString(),
            payer.Phone,
            payer.Website,
            payer.Status.ToString(),
            payer.CreatedAtUtc,
            payer.UpdatedAtUtc);
    }
}