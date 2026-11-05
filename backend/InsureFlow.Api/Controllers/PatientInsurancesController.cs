using InsureFlow.Application.PatientInsurances;
using InsureFlow.Contracts.PatientInsurances;
using Microsoft.AspNetCore.Mvc;

namespace InsureFlow.Api.Controllers;

[ApiController]
[Route("api/v1/patient-insurances")]
public sealed class PatientInsurancesController(
    IPatientInsuranceService service)
    : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(
        typeof(IReadOnlyList<PatientInsuranceDto>),
        StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<PatientInsuranceDto>>> Get(
        [FromQuery] long patientId,
        [FromQuery] PatientInsuranceListRequest request,
        CancellationToken cancellationToken)
    {
        if (patientId <= 0)
        {
            return BadRequest(
                new
                {
                    title = "Invalid patient ID.",
                    detail =
                        "patientId must be greater than zero."
                });
        }

        var result = await service.GetPagedAsync(
            patientId,
            request,
            cancellationToken);

        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(
        typeof(PatientInsuranceDto),
        StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PatientInsuranceDto>> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var result = await service.GetByIdAsync(
            id,
            cancellationToken);

        if (result is null)
        {
            return NotFound();
        }

        return Ok(result);
    }

    [HttpPost]
    [ProducesResponseType(
        typeof(PatientInsuranceDto),
        StatusCodes.Status201Created)]
    public async Task<ActionResult<PatientInsuranceDto>> Create(
        [FromBody] CreatePatientInsuranceRequest request,
        CancellationToken cancellationToken)
    {
        var result = await service.CreateAsync(
            request,
            cancellationToken);

        return CreatedAtAction(
            nameof(GetById),
            new { id = result.Id },
            result);
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(
        typeof(PatientInsuranceDto),
        StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PatientInsuranceDto>> Update(
        Guid id,
        [FromBody] UpdatePatientInsuranceRequest request,
        CancellationToken cancellationToken)
    {
        var result = await service.UpdateAsync(
            id,
            request,
            cancellationToken);

        if (result is null)
        {
            return NotFound();
        }

        return Ok(result);
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(
        Guid id,
        CancellationToken cancellationToken)
    {
        var deleted = await service.DeleteAsync(
            id,
            cancellationToken);

        if (!deleted)
        {
            return NotFound();
        }

        return NoContent();
    }

    [HttpPost("{id:guid}/activate")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Activate(
        Guid id,
        CancellationToken cancellationToken)
    {
        var updated = await service.ActivateAsync(
            id,
            cancellationToken);

        if (!updated)
        {
            return NotFound();
        }

        return NoContent();
    }

    [HttpPost("{id:guid}/deactivate")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Deactivate(
        Guid id,
        CancellationToken cancellationToken)
    {
        var updated = await service.DeactivateAsync(
            id,
            cancellationToken);

        if (!updated)
        {
            return NotFound();
        }

        return NoContent();
    }

    [HttpPost("{id:guid}/eligibility/verified")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> MarkEligibilityVerified(
        Guid id,
        CancellationToken cancellationToken)
    {
        var updated =
            await service.MarkEligibilityVerifiedAsync(
                id,
                cancellationToken);

        if (!updated)
        {
            return NotFound();
        }

        return NoContent();
    }

    [HttpPost("{id:guid}/eligibility/inactive")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> MarkEligibilityInactive(
        Guid id,
        CancellationToken cancellationToken)
    {
        var updated =
            await service.MarkEligibilityInactiveAsync(
                id,
                cancellationToken);

        if (!updated)
        {
            return NotFound();
        }

        return NoContent();
    }

    [HttpPost("{id:guid}/eligibility/error")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> MarkEligibilityError(
        Guid id,
        CancellationToken cancellationToken)
    {
        var updated =
            await service.MarkEligibilityErrorAsync(
                id,
                cancellationToken);

        if (!updated)
        {
            return NotFound();
        }

        return NoContent();
    }
}