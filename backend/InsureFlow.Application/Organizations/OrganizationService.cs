using InsureFlow.Application.Abstractions;
using InsureFlow.Application.Persistence;
using InsureFlow.Application.Tenants;
using InsureFlow.Contracts.Organizations;
using InsureFlow.Domain.Entities;
using InsureFlow.Domain.Enums;

namespace InsureFlow.Application.Organizations;

public sealed class OrganizationService : IOrganizationService
{
    private readonly IOrganizationRepository _repository;
    private readonly ITenantContext _tenantContext;
    private readonly IAuditService _auditService;

    public OrganizationService(
        IOrganizationRepository repository,
        ITenantContext tenantContext,
        IAuditService auditService)
    {
        _repository = repository;
        _tenantContext = tenantContext;
        _auditService = auditService;
    }

    public async Task<IReadOnlyList<OrganizationDto>> GetPagedAsync(
        OrganizationListRequest request,
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

        var organizations =
            await _repository.GetPagedAsync(
                request.Search,
                request.Status,
                page,
                pageSize,
                cancellationToken);

        return organizations
            .Select(MapToDto)
            .ToList();
    }

    public async Task<OrganizationDto?> GetByIdAsync(
        Guid externalId,
        CancellationToken cancellationToken = default)
    {
        if (externalId == Guid.Empty)
        {
            throw new ArgumentException(
                "Organization ID is required.",
                nameof(externalId));
        }

        var organization =
            await _repository.GetByExternalIdAsync(
                externalId,
                cancellationToken);

        return organization is null
            ? null
            : MapToDto(organization);
    }

    public async Task<OrganizationDto> CreateAsync(
        CreateOrganizationRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var tenantId = RequireTenant();

        var code = NormalizeRequired(
            request.Code,
            nameof(request.Code),
            "Organization code is required.");

        var name = NormalizeRequired(
            request.Name,
            nameof(request.Name),
            "Organization name is required.");

        if (!Enum.TryParse<OrganizationType>(
                request.Type,
                ignoreCase: true,
                out var organizationType))
        {
            throw new ArgumentException(
                "Invalid organization type.",
                nameof(request.Type));
        }

        if (await _repository.ExistsByCodeAsync(
                code,
                cancellationToken))
        {
            throw new InvalidOperationException(
                $"An organization with code '{code}' already exists.");
        }

        var organization = new Organization(
            tenantId,
            code,
            name,
            organizationType);

        organization.UpdateDetails(
            name,
            request.LegalName,
            request.Phone,
            request.Fax,
            request.Email,
            request.Website);

        await _repository.AddAsync(
            organization,
            cancellationToken);

        await _repository.SaveChangesAsync(
            cancellationToken);

        await _auditService.WriteAsync(
            new AuditEntry(
                TenantId: tenantId,
                UserId: null,
                Action: "OrganizationCreated",
                EntityName: nameof(Organization),
                EntityId: organization.ExternalId.ToString()),
            cancellationToken);

        return MapToDto(organization);
    }

    public async Task<bool> UpdateAsync(
        Guid externalId,
        UpdateOrganizationRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var tenantId = RequireTenant();

        var organization =
            await _repository.GetByExternalIdAsync(
                externalId,
                cancellationToken);

        if (organization is null)
        {
            return false;
        }

        organization.UpdateDetails(
            request.Name,
            request.LegalName,
            request.Phone,
            request.Fax,
            request.Email,
            request.Website);

        await _repository.UpdateAsync(
            organization,
            cancellationToken);

        await _repository.SaveChangesAsync(
            cancellationToken);

        await _auditService.WriteAsync(
            new AuditEntry(
                TenantId: tenantId,
                UserId: null,
                Action: "OrganizationUpdated",
                EntityName: nameof(Organization),
                EntityId: organization.ExternalId.ToString()),
            cancellationToken);

        return true;
    }

    public async Task<bool> DeactivateAsync(
        Guid externalId,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(externalId);

        var tenantId = RequireTenant();

        var organization =
            await _repository.GetByExternalIdAsync(
                externalId,
                cancellationToken);

        if (organization is null)
        {
            return false;
        }

        organization.Deactivate();

        await _repository.UpdateAsync(
            organization,
            cancellationToken);

        await _repository.SaveChangesAsync(
            cancellationToken);

        await _auditService.WriteAsync(
            new AuditEntry(
                TenantId: tenantId,
                UserId: null,
                Action: "OrganizationDeactivated",
                EntityName: nameof(Organization),
                EntityId: organization.ExternalId.ToString()),
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

    private static OrganizationDto MapToDto(
        Organization organization)
    {
        return new OrganizationDto(
            organization.ExternalId,
            organization.TenantId,
            organization.Code,
            organization.Name,
            organization.LegalName,
            organization.Type.ToString(),
            organization.TaxId,
            organization.Npi,
            organization.Phone,
            organization.Fax,
            organization.Email,
            organization.Website,
            organization.Status.ToString(),
            organization.CreatedAtUtc,
            organization.UpdatedAtUtc);
    }
}