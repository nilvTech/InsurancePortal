using InsureFlow.Application.InsurancePlans;
using InsureFlow.Contracts.InsurancePlans;
using Microsoft.AspNetCore.Mvc;

namespace InsureFlow.Api.Controllers;

[ApiController]
[Route("api/v1/insurance-plans")]
public sealed class InsurancePlansController : ControllerBase
{
    private readonly IInsurancePlanService _service;

    public InsurancePlansController(
        IInsurancePlanService service)
    {
        _service = service;
    }

    [HttpGet]
    [ProducesResponseType(
        typeof(IReadOnlyList<InsurancePlanDto>),
        StatusCodes.Status200OK)]
    public async Task<IActionResult> Get(
        [FromQuery] InsurancePlanListRequest request,
        CancellationToken cancellationToken)
    {
        var plans =
            await _service.GetPagedAsync(
                request,
                cancellationToken);

        return Ok(plans);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(
        typeof(InsurancePlanDto),
        StatusCodes.Status200OK)]
    [ProducesResponseType(
        StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var plan =
            await _service.GetByIdAsync(
                id,
                cancellationToken);

        if (plan is null)
        {
            return NotFound();
        }

        return Ok(plan);
    }

    [HttpPost]
    [ProducesResponseType(
        typeof(InsurancePlanDto),
        StatusCodes.Status201Created)]
    [ProducesResponseType(
        StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create(
        [FromBody] CreateInsurancePlanRequest request,
        CancellationToken cancellationToken)
    {
        var plan =
            await _service.CreateAsync(
                request,
                cancellationToken);

        return CreatedAtAction(
            nameof(GetById),
            new { id = plan.Id },
            plan);
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(
        StatusCodes.Status204NoContent)]
    [ProducesResponseType(
        StatusCodes.Status404NotFound)]
    [ProducesResponseType(
        StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Update(
        Guid id,
        [FromBody] UpdateInsurancePlanRequest request,
        CancellationToken cancellationToken)
    {
        var updated =
            await _service.UpdateAsync(
                id,
                request,
                cancellationToken);

        if (!updated)
        {
            return NotFound();
        }

        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(
        StatusCodes.Status204NoContent)]
    [ProducesResponseType(
        StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(
        Guid id,
        CancellationToken cancellationToken)
    {
        var deactivated =
            await _service.DeactivateAsync(
                id,
                cancellationToken);

        if (!deactivated)
        {
            return NotFound();
        }

        return NoContent();
    }

    [HttpPost("{id:guid}/activate")]
    [ProducesResponseType(
        StatusCodes.Status204NoContent)]
    [ProducesResponseType(
        StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Activate(
        Guid id,
        CancellationToken cancellationToken)
    {
        var activated =
            await _service.ActivateAsync(
                id,
                cancellationToken);

        if (!activated)
        {
            return NotFound();
        }

        return NoContent();
    }
}