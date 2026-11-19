
namespace insureflow_portal.Models.Dashboard;

public sealed class DashboardSummaryDto
{
    public int Eligibility { get; set; }
    public int Authorizations { get; set; }
    public int Claims { get; set; }
    public int Denials { get; set; }
    public int Appeals { get; set; }
    public int Payments { get; set; }
}

public sealed class DashboardStatusCountDto
{
    public string Status { get; set; } = string.Empty;
    public int Count { get; set; }
}

public class DashboardMetricDto
{
    public int Total { get; set; }
    public List<DashboardStatusCountDto> StatusBreakdown { get; set; } = [];
}

public sealed class DashboardDenialsDto : DashboardMetricDto
{
    public decimal TotalDeniedAmount { get; set; }
}

public sealed class DashboardPaymentsDto : DashboardMetricDto
{
    public decimal TotalPaymentAmount { get; set; }
    public decimal TotalAllocatedAmount { get; set; }
    public decimal TotalUnallocatedAmount { get; set; }
}

public sealed class DashboardPatientsDto
{
    public int Total { get; set; }
}

public sealed class DashboardInsuranceDto
{
    public int Total { get; set; }
    public int Verified { get; set; }
    public int Unverified { get; set; }
    public decimal VerificationPercentage { get; set; }
}

public sealed class DashboardEligibilityActivityDto
{
    public int Total { get; set; }
    public int Successful { get; set; }
    public int Failed { get; set; }
    public int Pending { get; set; }
    public decimal SuccessRate { get; set; }
    public List<DashboardTrendDto> Trend { get; set; } = [];
}

public sealed class DashboardTrendDto
{
    public string Day { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public int Value { get; set; }
}

public sealed class DashboardClaimsOverviewDto
{
    public int Total { get; set; }
    public List<DashboardClaimStatusDto> StatusBreakdown { get; set; } = [];
}

public sealed class DashboardClaimStatusDto
{
    public string Status { get; set; } = string.Empty;
    public int Count { get; set; }
    public decimal Percentage { get; set; }
}

public sealed class DashboardArAgingDto
{
    public decimal TotalOutstanding { get; set; }
    public int ClaimsWithBalance { get; set; }
    public List<DashboardAgingBucketDto> Aging { get; set; } = [];
}

public sealed class DashboardAgingBucketDto
{
    public string Label { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public decimal Percentage { get; set; }
}

public sealed class DashboardPayerDto
{
    public string Name { get; set; } = string.Empty;
    public int Claims { get; set; }
    public decimal PaidRate { get; set; }
    public decimal DenialRate { get; set; }
}

public sealed class DashboardRecentActivityDto
{
    public string Type { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
}
