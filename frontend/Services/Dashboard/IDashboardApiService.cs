
using insureflow_portal.Models.Dashboard;

namespace insureflow_portal.Services.Dashboard;

public interface IDashboardApiService
{
    Task<DashboardSummaryDto?> GetSummaryAsync();
    Task<DashboardMetricDto?> GetEligibilityAsync();
    Task<DashboardMetricDto?> GetAuthorizationsAsync();
    Task<DashboardMetricDto?> GetClaimsAsync();
    Task<DashboardDenialsDto?> GetDenialsAsync();
    Task<DashboardMetricDto?> GetAppealsAsync();
    Task<DashboardPaymentsDto?> GetPaymentsAsync();

    Task<DashboardPatientsDto?> GetPatientsAsync();
    Task<DashboardInsuranceDto?> GetInsuranceAsync();
    Task<DashboardEligibilityActivityDto?> GetEligibilityActivityAsync();
    Task<DashboardClaimsOverviewDto?> GetClaimsOverviewAsync();
    Task<DashboardArAgingDto?> GetArAgingAsync();
    Task<List<DashboardPayerDto>?> GetPayerPerformanceAsync();
    Task<List<DashboardRecentActivityDto>?> GetRecentActivityAsync();
}
