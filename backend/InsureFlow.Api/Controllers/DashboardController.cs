
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

    // GET /api/v1/dashboard/summary
    [HttpGet("summary")]
    public async Task<IActionResult> GetSummary(
        CancellationToken cancellationToken)
    {
        var result = new
        {
            Eligibility = await _db.Set<EligibilityRequest>()
                .CountAsync(cancellationToken),

            Authorizations = await _db.Set<Authorization>()
                .CountAsync(cancellationToken),

            Claims = await _db.Set<Claim>()
                .CountAsync(cancellationToken),

            Denials = await _db.Set<Denial>()
                .CountAsync(cancellationToken),

            Appeals = await _db.Set<Appeal>()
                .CountAsync(cancellationToken),

            Payments = await _db.Set<Payment>()
                .CountAsync(cancellationToken)
        };

        return Ok(result);
    }

    // GET /api/v1/dashboard/eligibility
    [HttpGet("eligibility")]
    public async Task<IActionResult> GetEligibility(
        CancellationToken cancellationToken)
    {
        var query = _db.Set<EligibilityRequest>().AsNoTracking();

        return Ok(new
        {
            Total = await query.CountAsync(cancellationToken),
            StatusBreakdown = await GetStatusBreakdown<EligibilityRequest>(
                cancellationToken)
        });
    }

    // GET /api/v1/dashboard/authorizations
    [HttpGet("authorizations")]
    public async Task<IActionResult> GetAuthorizations(
        CancellationToken cancellationToken)
    {
        var query = _db.Set<Authorization>().AsNoTracking();

        return Ok(new
        {
            Total = await query.CountAsync(cancellationToken),
            StatusBreakdown = await GetStatusBreakdown<Authorization>(
                cancellationToken)
        });
    }

    // GET /api/v1/dashboard/claims
    [HttpGet("claims")]
    public async Task<IActionResult> GetClaims(
        CancellationToken cancellationToken)
    {
        var query = _db.Set<Claim>().AsNoTracking();

        return Ok(new
        {
            Total = await query.CountAsync(cancellationToken),
            StatusBreakdown = await GetStatusBreakdown<Claim>(
                cancellationToken)
        });
    }

    // GET /api/v1/dashboard/denials
    [HttpGet("denials")]
    public async Task<IActionResult> GetDenials(
        CancellationToken cancellationToken)
    {
        var query = _db.Set<Denial>().AsNoTracking();

        return Ok(new
        {
            Total = await query.CountAsync(cancellationToken),

            TotalDeniedAmount = await query
                .SumAsync(x => x.DeniedAmount, cancellationToken),

            StatusBreakdown = await GetStatusBreakdown<Denial>(
                cancellationToken)
        });
    }

    // GET /api/v1/dashboard/appeals
    [HttpGet("appeals")]
    public async Task<IActionResult> GetAppeals(
        CancellationToken cancellationToken)
    {
        var query = _db.Set<Appeal>().AsNoTracking();

        return Ok(new
        {
            Total = await query.CountAsync(cancellationToken),
            StatusBreakdown = await GetStatusBreakdown<Appeal>(
                cancellationToken)
        });
    }

    // GET /api/v1/dashboard/payments
    [HttpGet("payments")]
    public async Task<IActionResult> GetPayments(
        CancellationToken cancellationToken)
    {
        var query = _db.Set<Payment>().AsNoTracking();

        return Ok(new
        {
            Total = await query.CountAsync(cancellationToken),

            TotalPaymentAmount = await query
                .SumAsync(x => x.PaymentAmount, cancellationToken),

            TotalAllocatedAmount = await query
                .SumAsync(x => x.AllocatedAmount, cancellationToken),

            TotalUnallocatedAmount = await query
                .SumAsync(x => x.UnallocatedAmount, cancellationToken),

            StatusBreakdown = await GetStatusBreakdown<Payment>(
                cancellationToken)
        });
    }

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

        return result
            .Select(x => (object)x)
            .ToList();
    }
}
