
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
            .AsNoTracking()
            .FirstOrDefaultAsync(
                x => x.ExternalId == roleId
                     && x.TenantId == TenantId
                     && x.IsActive,
                cancellationToken);

        if (role is null)
            return null;

        return await _db.RolePermissions
            .AsNoTracking()
            .Where(x => x.TenantId == TenantId && x.RoleId == role.Id)
            .Join(
                _db.Permissions.Where(
                    p => p.TenantId == TenantId && p.IsActive),
                rp => rp.PermissionId,
                p => p.Id,
                (rp, p) => new PermissionResponse
                {
                    ExternalId = p.ExternalId,
                    Code = p.Code,
                    Name = p.Name,
                    Description = p.Description
                })
            .OrderBy(x => x.Code)
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
            .Where(x => x.TenantId == TenantId && x.RoleId == role.Id)
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
                Description = x.Description
            })
            .ToList();
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
