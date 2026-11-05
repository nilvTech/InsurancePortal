using InsureFlow.Application.Tenants;

namespace InsureFlow.Api.Tenancy;

public sealed class HttpTenantContext(
    IHttpContextAccessor httpContextAccessor)
    : ITenantContext, ITenantContextAccessor
{
    private const string TenantIdItemKey =
        "InsureFlow.CurrentTenantId";

    public long? TenantId
    {
        get
        {
            var httpContext = httpContextAccessor.HttpContext;

            if (httpContext is null)
            {
                return null;
            }

            if (httpContext.Items.TryGetValue(
                    TenantIdItemKey,
                    out var value) &&
                value is long tenantId)
            {
                return tenantId;
            }

            return null;
        }
    }

    public bool HasTenant => TenantId.HasValue;

    public void SetTenant(long tenantId)
    {
        if (tenantId <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(tenantId),
                "Tenant ID must be greater than zero.");
        }

        var httpContext = httpContextAccessor.HttpContext;

        if (httpContext is null)
        {
            throw new InvalidOperationException(
                "Tenant context can only be set during an HTTP request.");
        }

        httpContext.Items[TenantIdItemKey] = tenantId;
    }

    public void Clear()
    {
        httpContextAccessor.HttpContext?
            .Items
            .Remove(TenantIdItemKey);
    }
}
