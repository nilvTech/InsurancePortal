
using InsureFlow.Application.Roles;
using InsureFlow.Application.Tenants;
using InsureFlow.Contracts.Roles;
using InsureFlow.Domain.Entities;
using InsureFlow.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace InsureFlow.Infrastructure.Roles;

public sealed class RoleService : IRoleService
{
    private readonly InsureFlowDbContext _db;
    private readonly ITenantContext _tenantContext;

    public RoleService(
        InsureFlowDbContext db,
        ITenantContext tenantContext)
    {
        _db = db;
        _tenantContext = tenantContext;
    }

    private long TenantId =>
        _tenantContext.TenantId
        ?? throw new InvalidOperationException(
            "A valid tenant context is required.");

    public async Task<IReadOnlyList<RoleResponse>> GetAllAsync(
        CancellationToken cancellationToken)
    {
        return await _db.Roles
            .AsNoTracking()
            .Where(x => x.TenantId == TenantId && x.IsActive)
            .OrderBy(x => x.Name)
            .Select(x => ToResponse(x))
            .ToListAsync(cancellationToken);
    }

    public async Task<RoleResponse?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        var role = await _db.Roles
            .AsNoTracking()
            .FirstOrDefaultAsync(
                x => x.ExternalId == id
                     && x.TenantId == TenantId
                     && x.IsActive,
                cancellationToken);

        return role is null ? null : ToResponse(role);
    }

    public async Task<RoleResponse> CreateAsync(
        CreateRoleRequest request,
        CancellationToken cancellationToken)
    {
        var name = request.Name?.Trim();

        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Role name is required.");

        var exists = await _db.Roles.AnyAsync(
            x => x.TenantId == TenantId
                 && x.Name.ToLower() == name.ToLower()
                 && x.IsActive,
            cancellationToken);

        if (exists)
            throw new InvalidOperationException(
                "A role with this name already exists.");

        var role = new Role(
            TenantId,
            name,
            request.Description);

        _db.Roles.Add(role);

        await _db.SaveChangesAsync(cancellationToken);

        return ToResponse(role);
    }

    public async Task<RoleResponse?> UpdateAsync(
        Guid id,
        UpdateRoleRequest request,
        CancellationToken cancellationToken)
    {
        var role = await _db.Roles.FirstOrDefaultAsync(
            x => x.ExternalId == id
                 && x.TenantId == TenantId
                 && x.IsActive,
            cancellationToken);

        if (role is null)
            return null;

        var name = request.Name?.Trim();

        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Role name is required.");

        var duplicate = await _db.Roles.AnyAsync(
            x => x.TenantId == TenantId
                 && x.Id != role.Id
                 && x.Name.ToLower() == name.ToLower()
                 && x.IsActive,
            cancellationToken);

        if (duplicate)
            throw new InvalidOperationException(
                "A role with this name already exists.");

        role.Update(name, request.Description);

        await _db.SaveChangesAsync(cancellationToken);

        return ToResponse(role);
    }

    public async Task<bool> DeleteAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        var role = await _db.Roles.FirstOrDefaultAsync(
            x => x.ExternalId == id
                 && x.TenantId == TenantId
                 && x.IsActive,
            cancellationToken);

        if (role is null)
            return false;

        role.Deactivate();

        await _db.SaveChangesAsync(cancellationToken);

        return true;
    }

    public async Task<IReadOnlyList<PermissionResponse>?> GetPermissionsAsync(
        Guid roleId,
        CancellationToken cancellationToken)
    {
        var role = await _db.Roles
            .FirstOrDefaultAsync(
                x => x.ExternalId == roleId
                     && x.TenantId == TenantId
                     && x.IsActive,
                cancellationToken);

        if (role is null)
            return null;

        await EnsureDefaultPermissionsAsync(cancellationToken);

        var assignedIds = await _db.RolePermissions
            .Where(x =>
                x.TenantId == TenantId &&
                x.RoleId == role.Id)
            .Select(x => x.PermissionId)
            .ToListAsync(cancellationToken);

        var assignedSet = assignedIds.ToHashSet();

        return await _db.Permissions
            .AsNoTracking()
            .Where(x =>
                x.TenantId == TenantId &&
                x.IsActive)
            .OrderBy(x => x.Code)
            .Select(x => new PermissionResponse
            {
                ExternalId = x.ExternalId,
                Code = x.Code,
                Name = x.Name,
                Description = x.Description,
                IsAssigned = assignedSet.Contains(x.Id)
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<PermissionResponse>?> UpdatePermissionsAsync(
        Guid roleId,
        UpdateRolePermissionsRequest request,
        CancellationToken cancellationToken)
    {
        var role = await _db.Roles.FirstOrDefaultAsync(
            x => x.ExternalId == roleId
                 && x.TenantId == TenantId
                 && x.IsActive,
            cancellationToken);

        if (role is null)
            return null;

        await EnsureDefaultPermissionsAsync(cancellationToken);

        var requestedIds = (request.PermissionIds ?? [])
            .Distinct()
            .ToList();

        var permissions = await _db.Permissions
            .Where(x =>
                x.TenantId == TenantId
                && x.IsActive
                && requestedIds.Contains(x.ExternalId))
            .ToListAsync(cancellationToken);

        if (permissions.Count != requestedIds.Count)
            throw new ArgumentException(
                "One or more permission IDs are invalid or inactive.");

        var existing = await _db.RolePermissions
            .Where(x =>
                x.TenantId == TenantId &&
                x.RoleId == role.Id)
            .ToListAsync(cancellationToken);

        _db.RolePermissions.RemoveRange(existing);

        foreach (var permission in permissions)
        {
            _db.RolePermissions.Add(
                new RolePermission(
                    TenantId,
                    role.Id,
                    permission.Id));
        }

        await _db.SaveChangesAsync(cancellationToken);

        return permissions
            .OrderBy(x => x.Code)
            .Select(x => new PermissionResponse
            {
                ExternalId = x.ExternalId,
                Code = x.Code,
                Name = x.Name,
                Description = x.Description,
                IsAssigned = true
            })
            .ToList();
    }

    private async Task EnsureDefaultPermissionsAsync(
        CancellationToken cancellationToken)
    {
        var defaults = new (string Code, string Name)[]
        {
            ("PATIENTS.VIEW", "View Patients"),
            ("PATIENTS.CREATE", "Create Patients"),
            ("PATIENTS.UPDATE", "Update Patients"),
            ("PATIENTS.DELETE", "Delete Patients"),

            ("INSURANCE_PLANS.VIEW", "View Insurance Plans"),
            ("INSURANCE_PLANS.CREATE", "Create Insurance Plans"),
            ("INSURANCE_PLANS.UPDATE", "Update Insurance Plans"),
            ("INSURANCE_PLANS.DELETE", "Delete Insurance Plans"),

            ("PATIENT_INSURANCE.VIEW", "View Patient Insurance"),
            ("PATIENT_INSURANCE.CREATE", "Create Patient Insurance"),
            ("PATIENT_INSURANCE.UPDATE", "Update Patient Insurance"),
            ("PATIENT_INSURANCE.DELETE", "Delete Patient Insurance"),

            ("ELIGIBILITY.VIEW", "View Eligibility"),
            ("ELIGIBILITY.CHECK", "Check Eligibility"),

            ("AUTHORIZATIONS.VIEW", "View Authorizations"),
            ("AUTHORIZATIONS.CREATE", "Create Authorizations"),
            ("AUTHORIZATIONS.UPDATE", "Update Authorizations"),
            ("AUTHORIZATIONS.SUBMIT", "Submit Authorizations"),
            ("AUTHORIZATIONS.APPROVE", "Approve Authorizations"),
            ("AUTHORIZATIONS.DENY", "Deny Authorizations"),

            ("CLAIMS.VIEW", "View Claims"),
            ("CLAIMS.CREATE", "Create Claims"),
            ("CLAIMS.UPDATE", "Update Claims"),
            ("CLAIMS.SUBMIT", "Submit Claims"),
            ("CLAIMS.RESUBMIT", "Resubmit Claims"),

            ("DENIALS.VIEW", "View Denials"),
            ("DENIALS.CREATE", "Create Denials"),
            ("DENIALS.UPDATE", "Update Denials"),
            ("DENIALS.ASSIGN", "Assign Denials"),
            ("DENIALS.RESOLVE", "Resolve Denials"),

            ("APPEALS.VIEW", "View Appeals"),
            ("APPEALS.CREATE", "Create Appeals"),
            ("APPEALS.UPDATE", "Update Appeals"),
            ("APPEALS.SUBMIT", "Submit Appeals"),

            ("PAYMENTS.VIEW", "View Payments"),
            ("PAYMENTS.CREATE", "Create Payments"),
            ("PAYMENTS.UPDATE", "Update Payments"),
            ("PAYMENTS.RECONCILE", "Reconcile Payments"),
            ("PAYMENTS.REFUND", "Process Refunds"),

            ("REPORTS.VIEW", "View Reports"),
            ("REPORTS.EXPORT", "Export Reports"),

            ("ORGANIZATIONS.VIEW", "View Organizations"),
            ("ORGANIZATIONS.CREATE", "Create Organizations"),
            ("ORGANIZATIONS.UPDATE", "Update Organizations"),

            ("PROVIDERS.VIEW", "View Providers"),
            ("PROVIDERS.CREATE", "Create Providers"),
            ("PROVIDERS.UPDATE", "Update Providers"),

            ("PAYERS.VIEW", "View Payers"),
            ("PAYERS.CREATE", "Create Payers"),
            ("PAYERS.UPDATE", "Update Payers"),

            ("SETTINGS.VIEW", "View Settings"),
            ("SETTINGS.UPDATE", "Update Settings")
        };

        var existingCodes = await _db.Permissions
            .Where(x => x.TenantId == TenantId)
            .Select(x => x.Code)
            .ToListAsync(cancellationToken);

        var existing = existingCodes.ToHashSet(
            StringComparer.OrdinalIgnoreCase);

        foreach (var item in defaults)
        {
            if (existing.Contains(item.Code))
                continue;

            _db.Permissions.Add(
                new Permission(
                    TenantId,
                    item.Code,
                    item.Name,
                    null));
        }

        if (_db.ChangeTracker.HasChanges())
        {
            await _db.SaveChangesAsync(cancellationToken);
        }
    }

    private static RoleResponse ToResponse(Role role)
    {
        return new RoleResponse
        {
            ExternalId = role.ExternalId,
            Name = role.Name,
            Description = role.Description,
            IsActive = role.IsActive,
            CreatedAtUtc = role.CreatedAtUtc,
            UpdatedAtUtc = role.UpdatedAtUtc
        };
    }
}
