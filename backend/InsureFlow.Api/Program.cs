using InsureFlow.Api.Configuration;
using InsureFlow.Api.ExceptionHandling;
using InsureFlow.Api.Middleware;
using InsureFlow.Application.DependencyInjection;
using InsureFlow.Infrastructure.DependencyInjection;
using InsureFlow.Infrastructure.Persistence;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using InsureFlow.Api.Tenancy;
using InsureFlow.Application.Tenants;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, services, configuration) => configuration
    .ReadFrom.Configuration(context.Configuration)
    .ReadFrom.Services(services)
    .Enrich.FromLogContext()
    .Enrich.WithProperty("Application", "InsureFlow.Api")
    .WriteTo.Console());

builder.Services.AddProblemDetails(options =>
{
    options.CustomizeProblemDetails = context =>
    {
        context.ProblemDetails.Extensions["traceId"] =
            context.HttpContext.TraceIdentifier;

        var correlationId =
            context.HttpContext.Items[
                CorrelationIdMiddleware.HeaderName]?.ToString();

        if (!string.IsNullOrWhiteSpace(correlationId))
        {
            context.ProblemDetails.Extensions["correlationId"] =
                correlationId;
        }
    };
});

builder.Services.AddExceptionHandler<InsureFlowExceptionHandler>();

builder.Services.AddControllers(options =>
{
    options.Filters.Add(
        new ProducesResponseTypeAttribute(
            StatusCodes.Status500InternalServerError));
});
builder.Services.AddHttpContextAccessor();

builder.Services.AddScoped<
    HttpTenantContext>();

builder.Services.AddScoped<
    ITenantContext>(sp =>
        sp.GetRequiredService<HttpTenantContext>());

builder.Services.AddScoped<
    ITenantContextAccessor>(sp =>
        sp.GetRequiredService<HttpTenantContext>());

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddInsureFlowApiVersioning();

builder.Services.AddInsureFlowApplication();

builder.Services.AddInsureFlowInfrastructure(
    builder.Configuration);

builder.Services.AddHealthChecks()
    .AddCheck(
        "api",
        () => Microsoft.Extensions.Diagnostics.HealthChecks
            .HealthCheckResult.Healthy(),
        tags: ["ready"])
    .AddDbContextCheck<InsureFlowDbContext>(
        "database",
        tags: ["ready", "database"]);

var app = builder.Build();

app.UseMiddleware<CorrelationIdMiddleware>();

app.UseExceptionHandler();

app.UseSerilogRequestLogging();

app.UseMiddleware<TenantResolutionMiddleware>();

app.UseSwagger();

app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.MapHealthChecks(
    "/health",
    new HealthCheckOptions());

app.MapHealthChecks(
    "/health/ready",
    new HealthCheckOptions
    {
        Predicate = check => check.Tags.Contains("ready")
    });

app.Run();

public partial class Program { }