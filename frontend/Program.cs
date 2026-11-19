
using insureflow_portal.Components;
using insureflow_portal.Configuration;
using insureflow_portal.Services.Api;
using insureflow_portal.Services.Dashboard;
var builder = WebApplication.CreateBuilder(args);

// API configuration
builder.Services
    .AddOptions<ApiSettings>()
    .Bind(builder.Configuration.GetSection(ApiSettings.SectionName))
    .Validate(
        settings => Uri.TryCreate(
            settings.BaseUrl,
            UriKind.Absolute,
            out var uri) &&
            (uri.Scheme == Uri.UriSchemeHttp ||
             uri.Scheme == Uri.UriSchemeHttps),
        "ApiSettings:BaseUrl must be a valid HTTP or HTTPS URL.")
    .Validate(
        settings => settings.TenantId is null ||
                    settings.TenantId > 0,
        "ApiSettings:TenantId must be a positive number.")
    .ValidateOnStart();

// Shared API client
builder.Services.AddHttpClient<IApiClient, ApiClient>(
    (serviceProvider, httpClient) =>
    {
        var settings = serviceProvider
            .GetRequiredService<
                Microsoft.Extensions.Options.IOptions<ApiSettings>>()
            .Value;

        httpClient.BaseAddress = new Uri(
            settings.BaseUrl.TrimEnd('/') + "/");

        if (settings.TenantId.HasValue)
        {
            httpClient.DefaultRequestHeaders.Remove("X-Tenant-ID");

            httpClient.DefaultRequestHeaders.Add(
                "X-Tenant-ID",
                settings.TenantId.Value.ToString());
        }
    });
builder.Services.AddScoped<IDashboardApiService, DashboardApiService>();
// Blazor services
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseStatusCodePagesWithReExecute(
    "/not-found",
    createScopeForStatusCodePages: true);

app.UseHttpsRedirection();
app.UseAntiforgery();

app.MapStaticAssets();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
