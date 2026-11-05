using InsureFlow.Api.Tenancy;

namespace InsureFlow.Api.Middleware;

public sealed class TenantResolutionMiddleware(
    RequestDelegate next)
{
    public const string HeaderName = "X-Tenant-ID";

    public async Task InvokeAsync(
        HttpContext context,
        ITenantContextAccessor tenantContextAccessor,
        IWebHostEnvironment environment)
    {
        if (!environment.IsDevelopment())
        {
            await next(context);
            return;
        }

        if (!context.Request.Headers.TryGetValue(
                HeaderName,
                out var headerValue) ||
            string.IsNullOrWhiteSpace(headerValue))
        {
            await next(context);
            return;
        }

        if (!long.TryParse(
                headerValue.ToString(),
                out var tenantId) ||
            tenantId <= 0)
        {
            context.Response.StatusCode =
                StatusCodes.Status400BadRequest;

            await context.Response.WriteAsJsonAsync(
                new
                {
                    title = "Invalid tenant ID.",
                    status = 400,
                    detail =
                        "The X-Tenant-ID header must contain a positive integer."
                });

            return;
        }

        tenantContextAccessor.SetTenant(tenantId);

        await next(context);
    }
}
