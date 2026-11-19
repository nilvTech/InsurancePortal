
using insureflow_portal.Models.Dashboard;
using insureflow_portal.Services.Api;

namespace insureflow_portal.Services.Dashboard;

public sealed class DashboardApiService(IApiClient apiClient)
    : IDashboardApiService
{
    private const string BaseEndpoint = "api/v1/dashboard";

    public Task<DashboardSummaryDto?> GetSummaryAsync() =>
        apiClient.GetAsync<DashboardSummaryDto>($"{BaseEndpoint}/summary");

    public Task<DashboardMetricDto?> GetEligibilityAsync() =>
        apiClient.GetAsync<DashboardMetricDto>($"{BaseEndpoint}/eligibility");

    public Task<DashboardMetricDto?> GetAuthorizationsAsync() =>
        apiClient.GetAsync<DashboardMetricDto>($"{BaseEndpoint}/authorizations");

    public Task<DashboardMetricDto?> GetClaimsAsync() =>
        apiClient.GetAsync<DashboardMetricDto>($"{BaseEndpoint}/claims");

    public Task<DashboardDenialsDto?> GetDenialsAsync() =>
        apiClient.GetAsync<DashboardDenialsDto>($"{BaseEndpoint}/denials");

    public Task<DashboardMetricDto?> GetAppealsAsync() =>
        apiClient.GetAsync<DashboardMetricDto>($"{BaseEndpoint}/appeals");

    public Task<DashboardPaymentsDto?> GetPaymentsAsync() =>
        apiClient.GetAsync<DashboardPaymentsDto>($"{BaseEndpoint}/payments");

    public Task<DashboardPatientsDto?> GetPatientsAsync() =>
        apiClient.GetAsync<DashboardPatientsDto>($"{BaseEndpoint}/patients");

    public Task<DashboardInsuranceDto?> GetInsuranceAsync() =>
        apiClient.GetAsync<DashboardInsuranceDto>($"{BaseEndpoint}/insurance");

    public Task<DashboardEligibilityActivityDto?> GetEligibilityActivityAsync() =>
        apiClient.GetAsync<DashboardEligibilityActivityDto>(
            $"{BaseEndpoint}/eligibility-activity");

    public Task<DashboardClaimsOverviewDto?> GetClaimsOverviewAsync() =>
        apiClient.GetAsync<DashboardClaimsOverviewDto>(
            $"{BaseEndpoint}/claims-overview");

    public Task<DashboardArAgingDto?> GetArAgingAsync() =>
        apiClient.GetAsync<DashboardArAgingDto>(
            $"{BaseEndpoint}/ar-aging");

    public Task<List<DashboardPayerDto>?> GetPayerPerformanceAsync() =>
        apiClient.GetAsync<List<DashboardPayerDto>>(
            $"{BaseEndpoint}/payer-performance");

    public Task<List<DashboardRecentActivityDto>?> GetRecentActivityAsync() =>
        apiClient.GetAsync<List<DashboardRecentActivityDto>>(
            $"{BaseEndpoint}/recent-activity");
}
