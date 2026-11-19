namespace insureflow_portal.Configuration;

public sealed class ApiSettings
{
    public const string SectionName = "ApiSettings";

    public string BaseUrl { get; set; } = "http://localhost:5228/";

    public long? TenantId { get; set; }
}