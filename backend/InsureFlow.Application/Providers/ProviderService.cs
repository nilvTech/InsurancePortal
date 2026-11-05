using InsureFlow.Application.Abstractions;
using InsureFlow.Application.Persistence;
using InsureFlow.Application.Tenants;
using InsureFlow.Contracts.Providers;
using InsureFlow.Domain.Entities;

namespace InsureFlow.Application.Providers;

public sealed class ProviderService : IProviderService
{
    private readonly IProviderRepository _providerRepository;
    private readonly IOrganizationRepository _organizationRepository;
    private readonly ITenantContext _tenantContext;
    private readonly IAuditService _auditService;

    public ProviderService(
        IProviderRepository providerRepository,
        IOrganizationRepository organizationRepository,
        ITenantContext tenantContext,
        IAuditService auditService)
    {
        _providerRepository = providerRepository;
        _organizationRepository = organizationRepository;
        _tenantContext = tenantContext;
        _auditService = auditService;
    }

    public async Task<IReadOnlyList<ProviderDto>> GetPagedAsync(
        ProviderListRequest request,
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

        long? organizationId = null;

        if (request.OrganizationId.HasValue)
        {
            var organization =
                await _organizationRepository.GetByExternalIdAsync(
                    request.OrganizationId.Value,
                    cancellationToken);

            if (organization is null)
            {
                return [];
            }

            organizationId = organization.Id;
        }

        var providers =
            await _providerRepository.GetPagedAsync(
                request.Search,
                request.Status,
                organizationId,
                page,
                pageSize,
                cancellationToken);

        return providers
            .Select(MapToDto)
            .ToList();
    }

    public async Task<ProviderDto?> GetByIdAsync(
        Guid externalId,
        CancellationToken cancellationToken = default)
    {
        if (externalId == Guid.Empty)
        {
            throw new ArgumentException(
                "Provider ID is required.",
                nameof(externalId));
        }

        var provider =
            await _providerRepository.GetByExternalIdAsync(
                externalId,
                cancellationToken);

        return provider is null
            ? null
            : MapToDto(provider);
    }

    public async Task<ProviderDto> CreateAsync(
        CreateProviderRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var tenantId = RequireTenant();

        if (request.OrganizationId == Guid.Empty)
        {
            throw new ArgumentException(
                "Organization ID is required.",
                nameof(request.OrganizationId));
        }

        var organization =
            await _organizationRepository.GetByExternalIdAsync(
                request.OrganizationId,
                cancellationToken);

        if (organization is null)
        {
            throw new InvalidOperationException(
                "The specified organization was not found " +
                "for the current tenant.");
        }

        var code = NormalizeRequired(
            request.Code,
            nameof(request.Code),
            "Provider code is required.");

        var firstName = NormalizeRequired(
            request.FirstName,
            nameof(request.FirstName),
            "Provider first name is required.");

        var lastName = NormalizeRequired(
            request.LastName,
            nameof(request.LastName),
            "Provider last name is required.");

        var npi = NormalizeRequired(
            request.Npi,
            nameof(request.Npi),
            "Provider NPI is required.");

        if (npi.Length != 10 ||
            !npi.All(char.IsDigit))
        {
            throw new ArgumentException(
                "Provider NPI must contain exactly 10 digits.",
                nameof(request.Npi));
        }

        if (await _providerRepository.ExistsByCodeAsync(
                code,
                cancellationToken))
        {
            throw new InvalidOperationException(
                $"A provider with code '{code}' already exists.");
        }

        if (await _providerRepository.ExistsByNpiAsync(
                npi,
                cancellationToken))
        {
            throw new InvalidOperationException(
                $"A provider with NPI '{npi}' already exists.");
        }

        var provider = new Provider(
            tenantId,
            organization.Id,
            code,
            firstName,
            lastName,
            npi);

        provider.UpdateDetails(
            firstName,
            request.MiddleName,
            lastName,
            request.Credential,
            request.Specialty,
            request.Department,
            request.Phone,
            request.Email);

        await _providerRepository.AddAsync(
            provider,
            cancellationToken);

        await _providerRepository.SaveChangesAsync(
            cancellationToken);

        await _auditService.WriteAsync(
            new AuditEntry(
                TenantId: tenantId,
                UserId: null,
                Action: "ProviderCreated",
                EntityName: nameof(Provider),
                EntityId: provider.ExternalId.ToString()),
            cancellationToken);

        return MapToDto(provider);
    }

    public async Task<bool> UpdateAsync(
        Guid externalId,
        UpdateProviderRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var tenantId = RequireTenant();

        var provider =
            await _providerRepository.GetByExternalIdAsync(
                externalId,
                cancellationToken);

        if (provider is null)
        {
            return false;
        }

        provider.UpdateDetails(
            request.FirstName,
            request.MiddleName,
            request.LastName,
            request.Credential,
            request.Specialty,
            request.Department,
            request.Phone,
            request.Email);

        await _providerRepository.UpdateAsync(
            provider,
            cancellationToken);

        await _providerRepository.SaveChangesAsync(
            cancellationToken);

        await _auditService.WriteAsync(
            new AuditEntry(
                TenantId: tenantId,
                UserId: null,
                Action: "ProviderUpdated",
                EntityName: nameof(Provider),
                EntityId: provider.ExternalId.ToString()),
            cancellationToken);

        return true;
    }

    public async Task<bool> DeactivateAsync(
        Guid externalId,
        CancellationToken cancellationToken = default)
    {
        var tenantId = RequireTenant();

        var provider =
            await _providerRepository.GetByExternalIdAsync(
                externalId,
                cancellationToken);

        if (provider is null)
        {
            return false;
        }

        provider.Deactivate();

        await _providerRepository.UpdateAsync(
            provider,
            cancellationToken);

        await _providerRepository.SaveChangesAsync(
            cancellationToken);

        await _auditService.WriteAsync(
            new AuditEntry(
                TenantId: tenantId,
                UserId: null,
                Action: "ProviderDeactivated",
                EntityName: nameof(Provider),
                EntityId: provider.ExternalId.ToString()),
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

    private static ProviderDto MapToDto(
        Provider provider)
    {
        return new ProviderDto(
            provider.ExternalId,
            provider.TenantId,
            provider.Organization.ExternalId,
            provider.Code,
            provider.FirstName,
            provider.MiddleName,
            provider.LastName,
            provider.Credential,
            provider.Npi,
            provider.Specialty,
            provider.Department,
            provider.Phone,
            provider.Email,
            provider.Status.ToString(),
            provider.CreatedAtUtc,
            provider.UpdatedAtUtc);
    }
}