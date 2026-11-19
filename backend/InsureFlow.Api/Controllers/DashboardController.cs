
using InsureFlow.Domain.Entities;
using InsureFlow.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InsureFlow.Api.Controllers;

[ApiController]
[Route("api/v1/dashboard")]
public class DashboardController : ControllerBase
{
    private readonly InsureFlowDbContext _db;

    public DashboardController(InsureFlowDbContext db)
    {
        _db = db;
    }

    // =========================================================
    // EXISTING DASHBOARD ENDPOINTS
    // =========================================================

    [HttpGet("summary")]
    public async Task<IActionResult> GetSummary(CancellationToken cancellationToken)
    {
        return Ok(new
        {
            Eligibility = await _db.Set<EligibilityRequest>().CountAsync(cancellationToken),
            Authorizations = await _db.Set<Authorization>().CountAsync(cancellationToken),
            Claims = await _db.Set<Claim>().CountAsync(cancellationToken),
            Denials = await _db.Set<Denial>().CountAsync(cancellationToken),
            Appeals = await _db.Set<Appeal>().CountAsync(cancellationToken),
            Payments = await _db.Set<Payment>().CountAsync(cancellationToken)
        });
    }

    [HttpGet("eligibility")]
    public async Task<IActionResult> GetEligibility(CancellationToken cancellationToken)
    {
        var statuses = await _db.Set<EligibilityRequest>()
            .AsNoTracking()
            .Select(x => x.Status)
            .ToListAsync(cancellationToken);

        return Ok(new
        {
            Total = statuses.Count,
            StatusBreakdown = statuses
                .GroupBy(x => x)
                .Select(x => new
                {
                    Status = x.Key.ToString(),
                    Count = x.Count()
                })
                .ToList()
        });
    }

    [HttpGet("authorizations")]
    public async Task<IActionResult> GetAuthorizations(CancellationToken cancellationToken)
    {
        return Ok(new
        {
            Total = await _db.Set<Authorization>().CountAsync(cancellationToken),
            StatusBreakdown = await GetStatusBreakdown<Authorization>(cancellationToken)
        });
    }

    [HttpGet("claims")]
    public async Task<IActionResult> GetClaims(CancellationToken cancellationToken)
    {
        return Ok(new
        {
            Total = await _db.Set<Claim>().CountAsync(cancellationToken),
            StatusBreakdown = await GetStatusBreakdown<Claim>(cancellationToken)
        });
    }

    [HttpGet("denials")]
    public async Task<IActionResult> GetDenials(CancellationToken cancellationToken)
    {
        var query = _db.Set<Denial>().AsNoTracking();

        return Ok(new
        {
            Total = await query.CountAsync(cancellationToken),
            TotalDeniedAmount = await query.SumAsync(
                x => (decimal?)x.DeniedAmount, cancellationToken) ?? 0m,
            StatusBreakdown = await GetStatusBreakdown<Denial>(cancellationToken)
        });
    }

    [HttpGet("appeals")]
    public async Task<IActionResult> GetAppeals(CancellationToken cancellationToken)
    {
        return Ok(new
        {
            Total = await _db.Set<Appeal>().CountAsync(cancellationToken),
            StatusBreakdown = await GetStatusBreakdown<Appeal>(cancellationToken)
        });
    }

    [HttpGet("payments")]
    public async Task<IActionResult> GetPayments(CancellationToken cancellationToken)
    {
        var query = _db.Set<Payment>().AsNoTracking();

        return Ok(new
        {
            Total = await query.CountAsync(cancellationToken),
            TotalPaymentAmount = await query.SumAsync(
                x => (decimal?)x.PaymentAmount, cancellationToken) ?? 0m,
            TotalAllocatedAmount = await query.SumAsync(
                x => (decimal?)x.AllocatedAmount, cancellationToken) ?? 0m,
            TotalUnallocatedAmount = await query.SumAsync(
                x => (decimal?)x.UnallocatedAmount, cancellationToken) ?? 0m,
            StatusBreakdown = await GetStatusBreakdown<Payment>(cancellationToken)
        });
    }

    // =========================================================
    // PATIENTS
    // =========================================================

    [HttpGet("patients")]
    public async Task<IActionResult> GetPatients(CancellationToken cancellationToken)
    {
        var total = await _db.Set<Patient>()
            .AsNoTracking()
            .CountAsync(cancellationToken);

        return Ok(new
        {
            Total = total
        });
    }

    // =========================================================
    // INSURANCE VERIFICATION
    // =========================================================

    [HttpGet("insurance")]
    public async Task<IActionResult> GetInsurance(CancellationToken cancellationToken)
    {
        var statuses = await _db.Set<PatientInsurance>()
            .AsNoTracking()
            .Select(x => x.EligibilityStatus)
            .ToListAsync(cancellationToken);

        var total = statuses.Count;
        var verified = statuses.Count(x =>
            x.ToString().Equals("Verified", StringComparison.OrdinalIgnoreCase));

        var percentage = total == 0
            ? 0
            : Math.Round((decimal)verified / total * 100, 1);

        return Ok(new
        {
            Total = total,
            Verified = verified,
            Unverified = total - verified,
            VerificationPercentage = percentage
        });
    }

    // =========================================================
    // ELIGIBILITY ACTIVITY
    // =========================================================

    [HttpGet("eligibility-activity")]
    public async Task<IActionResult> GetEligibilityActivity(
        CancellationToken cancellationToken)
    {
        var requests = await _db.Set<EligibilityRequest>()
            .AsNoTracking()
            .Select(x => new
            {
                x.Status,
                x.RequestedAt
            })
            .ToListAsync(cancellationToken);

        var successful = requests.Count(x =>
            x.Status.ToString().Equals("Completed", StringComparison.OrdinalIgnoreCase));

        var failed = requests.Count(x =>
            x.Status.ToString().Equals("Failed", StringComparison.OrdinalIgnoreCase));

        var pending = requests.Count(x =>
            x.Status.ToString() is "Pending" or "Processing");

        var today = DateTime.UtcNow.Date;
        var startDate = today.AddDays(-6);

        var dailyActivity = requests
            .Where(x => x.RequestedAt.Date >= startDate)
            .GroupBy(x => x.RequestedAt.Date)
            .Select(x => new
            {
                Date = x.Key,
                Count = x.Count()
            })
            .ToDictionary(x => x.Date, x => x.Count);

        var trend = Enumerable.Range(0, 7)
            .Select(offset =>
            {
                var date = startDate.AddDays(offset);

                return new
                {
                    Day = date.ToString("ddd"),
                    Date = date,
                    Value = dailyActivity.GetValueOrDefault(date, 0)
                };
            })
            .ToList();

        var total = requests.Count;
        var successRate = total == 0
            ? 0
            : Math.Round((decimal)successful / total * 100, 1);

        return Ok(new
        {
            Total = total,
            Successful = successful,
            Failed = failed,
            Pending = pending,
            SuccessRate = successRate,
            Trend = trend
        });
    }

    // =========================================================
    // CLAIMS OVERVIEW
    // =========================================================

    [HttpGet("claims-overview")]
    public async Task<IActionResult> GetClaimsOverview(
        CancellationToken cancellationToken)
    {
        var statuses = await _db.Set<Claim>()
            .AsNoTracking()
            .Select(x => x.Status)
            .ToListAsync(cancellationToken);

        var total = statuses.Count;

        var breakdown = statuses
            .GroupBy(x => x.ToUpperInvariant())
            .Select(x => new
            {
                Status = x.Key,
                Count = x.Count(),
                Percentage = total == 0
                    ? 0
                    : Math.Round((decimal)x.Count() / total * 100, 1)
            })
            .OrderByDescending(x => x.Count)
            .ToList();

        return Ok(new
        {
            Total = total,
            StatusBreakdown = breakdown
        });
    }

    // =========================================================
    // ACCOUNTS RECEIVABLE AGING
    // =========================================================

    [HttpGet("ar-aging")]
    public async Task<IActionResult> GetArAging(
        CancellationToken cancellationToken)
    {
        var claims = await _db.Set<Claim>()
            .AsNoTracking()
            .Where(x => x.Status != "PAID" && x.Status != "CANCELLED")
            .Select(x => new
            {
                x.Id,
                x.TotalChargeAmount,
                AgingDate = x.SubmittedAtUtc ?? x.CreatedAtUtc
            })
            .ToListAsync(cancellationToken);

        var allocations = await (
            from allocation in _db.Set<PaymentAllocation>().AsNoTracking()
            join payment in _db.Set<Payment>().AsNoTracking()
                on allocation.PaymentId equals payment.Id
            where allocation.Status == "ACTIVE"
                  && payment.PostedAtUtc != null
                  && payment.ReversedAtUtc == null
            group allocation by allocation.ClaimId
            into grouped
            select new
            {
                ClaimId = grouped.Key,
                AllocatedAmount = grouped.Sum(x => x.AllocatedAmount)
            })
            .ToListAsync(cancellationToken);

        var allocatedByClaim = allocations.ToDictionary(
            x => x.ClaimId,
            x => x.AllocatedAmount);

        var today = DateTime.UtcNow.Date;

        var buckets = new[]
        {
            new { Label = "0-30 Days", Minimum = 0, Maximum = 30 },
            new { Label = "31-60 Days", Minimum = 31, Maximum = 60 },
            new { Label = "61-90 Days", Minimum = 61, Maximum = 90 },
            new { Label = "90+ Days", Minimum = 91, Maximum = int.MaxValue }
        };

        var outstandingClaims = claims
            .Select(claim =>
            {
                var allocated = allocatedByClaim.GetValueOrDefault(claim.Id, 0m);
                var outstanding = Math.Max(0m, claim.TotalChargeAmount - allocated);
                var age = Math.Max(0, (today - claim.AgingDate.Date).Days);

                return new
                {
                    Outstanding = outstanding,
                    Age = age
                };
            })
            .Where(x => x.Outstanding > 0)
            .ToList();

        var totalOutstanding = outstandingClaims.Sum(x => x.Outstanding);

        var aging = buckets.Select(bucket =>
        {
            var amount = outstandingClaims
                .Where(x => x.Age >= bucket.Minimum && x.Age <= bucket.Maximum)
                .Sum(x => x.Outstanding);

            return new
            {
                bucket.Label,
                Amount = amount,
                Percentage = totalOutstanding == 0
                    ? 0
                    : Math.Round(amount / totalOutstanding * 100, 1)
            };
        }).ToList();

        return Ok(new
        {
            TotalOutstanding = totalOutstanding,
            ClaimsWithBalance = outstandingClaims.Count,
            Aging = aging
        });
    }

    // =========================================================
    // PAYER PERFORMANCE
    // =========================================================

    [HttpGet("payer-performance")]
    public async Task<IActionResult> GetPayerPerformance(
        CancellationToken cancellationToken)
    {
        var data = await (
            from claim in _db.Set<Claim>().AsNoTracking()
            join payer in _db.Set<Payer>().AsNoTracking()
                on claim.PayerId equals payer.Id
            select new
            {
                Payer = payer.Name,
                claim.Status
            })
            .ToListAsync(cancellationToken);

        var result = data
            .GroupBy(x => x.Payer)
            .Select(group =>
            {
                var total = group.Count();
                var paid = group.Count(x =>
                    x.Status.Equals("PAID", StringComparison.OrdinalIgnoreCase));
                var denied = group.Count(x =>
                    x.Status.Equals("DENIED", StringComparison.OrdinalIgnoreCase));

                return new
                {
                    Name = group.Key,
                    Claims = total,
                    PaidRate = total == 0
                        ? 0
                        : Math.Round((decimal)paid / total * 100, 1),
                    DenialRate = total == 0
                        ? 0
                        : Math.Round((decimal)denied / total * 100, 1)
                };
            })
            .OrderByDescending(x => x.Claims)
            .ToList();

        return Ok(result);
    }

    // =========================================================
    // RECENT ACTIVITY
    // =========================================================

    [HttpGet("recent-activity")]
    public async Task<IActionResult> GetRecentActivity(
        CancellationToken cancellationToken)
    {
        var activities = new List<DashboardActivity>();

        var claimHistory = await _db.Set<ClaimHistory>()
            .AsNoTracking()
            .OrderByDescending(x => x.ChangedAt)
            .Take(15)
            .Select(x => new
            {
                x.ToStatus,
                x.Note,
                x.ChangedAt
            })
            .ToListAsync(cancellationToken);

        activities.AddRange(claimHistory.Select(x => new DashboardActivity(
            "Claim",
            $"Claim status changed to {x.ToStatus}",
            x.Note ?? "Claim status updated",
            x.ChangedAt)));

        var eligibilityHistory = await _db.Set<EligibilityHistory>()
            .AsNoTracking()
            .OrderByDescending(x => x.ChangedAt)
            .Take(15)
            .Select(x => new
            {
                x.ToStatus,
                x.Reason,
                x.ChangedAt
            })
            .ToListAsync(cancellationToken);

        activities.AddRange(eligibilityHistory.Select(x => new DashboardActivity(
            "Eligibility",
            $"Eligibility status changed to {x.ToStatus}",
            x.Reason ?? "Eligibility request updated",
            x.ChangedAt)));

        var authorizationHistory = await _db.Set<AuthorizationHistory>()
            .AsNoTracking()
            .OrderByDescending(x => x.ChangedAt)
            .Take(15)
            .Select(x => new
            {
                x.ToStatus,
                x.Note,
                x.ChangedAt
            })
            .ToListAsync(cancellationToken);

        activities.AddRange(authorizationHistory.Select(x => new DashboardActivity(
            "Authorization",
            $"Authorization status changed to {x.ToStatus}",
            x.Note ?? "Authorization updated",
            x.ChangedAt)));

        var denialActivities = await _db.Set<DenialActivity>()
            .AsNoTracking()
            .OrderByDescending(x => x.ActivityDateUtc)
            .Take(15)
            .Select(x => new
            {
                x.ActivityType,
                x.Description,
                x.ActivityDateUtc
            })
            .ToListAsync(cancellationToken);

        activities.AddRange(denialActivities.Select(x => new DashboardActivity(
            "Denial",
            x.ActivityType,
            x.Description,
            x.ActivityDateUtc)));

        var appealActivities = await _db.Set<AppealHistory>()
            .AsNoTracking()
            .OrderByDescending(x => x.ActivityDateUtc)
            .Take(15)
            .Select(x => new
            {
                x.ActivityType,
                x.Description,
                x.ActivityDateUtc
            })
            .ToListAsync(cancellationToken);

        activities.AddRange(appealActivities.Select(x => new DashboardActivity(
            "Appeal",
            x.ActivityType,
            x.Description,
            x.ActivityDateUtc)));

        var result = activities
            .OrderByDescending(x => x.Timestamp)
            .Take(15)
            .ToList();

        return Ok(result);
    }

    // =========================================================
    // SHARED HELPERS
    // =========================================================

    private async Task<List<object>> GetStatusBreakdown<TEntity>(
        CancellationToken cancellationToken)
        where TEntity : class
    {
        var result = await _db.Set<TEntity>()
            .AsNoTracking()
            .GroupBy(x => EF.Property<string>(x, "Status"))
            .Select(group => new
            {
                Status = group.Key,
                Count = group.Count()
            })
            .ToListAsync(cancellationToken);

        return result.Select(x => (object)x).ToList();
    }

    private sealed record DashboardActivity(
        string Type,
        string Title,
        string Description,
        DateTime Timestamp);
}
