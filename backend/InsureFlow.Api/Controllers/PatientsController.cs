using InsureFlow.Application.Patients;
using InsureFlow.Contracts.Patients;
using Microsoft.AspNetCore.Mvc;

namespace InsureFlow.Api.Controllers;

[ApiController]
[Route("api/v1/patients")]
public sealed class PatientsController : ControllerBase
{
    private readonly IPatientService _patientService;

    public PatientsController(
        IPatientService patientService)
    {
        _patientService = patientService;
    }

    [HttpGet]
    [ProducesResponseType(
        typeof(IReadOnlyList<PatientDto>),
        StatusCodes.Status200OK)]
    public async Task<IActionResult> Get(
        [FromQuery] PatientListRequest request,
        CancellationToken cancellationToken)
    {
        var patients =
            await _patientService.GetPagedAsync(
                request,
                cancellationToken);

        return Ok(patients);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(
        typeof(PatientDto),
        StatusCodes.Status200OK)]
    [ProducesResponseType(
        StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var patient =
            await _patientService.GetByIdAsync(
                id,
                cancellationToken);

        if (patient is null)
        {
            return NotFound();
        }

        return Ok(patient);
    }

    [HttpPost]
    [ProducesResponseType(
        typeof(PatientDto),
        StatusCodes.Status201Created)]
    [ProducesResponseType(
        StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create(
        [FromBody] CreatePatientRequest request,
        CancellationToken cancellationToken)
    {
        var patient =
            await _patientService.CreateAsync(
                request,
                cancellationToken);

        return CreatedAtAction(
            nameof(GetById),
            new { id = patient.Id },
            patient);
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
        [FromBody] UpdatePatientRequest request,
        CancellationToken cancellationToken)
    {
        var updated =
            await _patientService.UpdateAsync(
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
            await _patientService.DeactivateAsync(
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
            await _patientService.ActivateAsync(
                id,
                cancellationToken);

        if (!activated)
        {
            return NotFound();
        }

        return NoContent();
    }

    [HttpPost("{id:guid}/portal-access")]
    [ProducesResponseType(
        StatusCodes.Status204NoContent)]
    [ProducesResponseType(
        StatusCodes.Status404NotFound)]
    [ProducesResponseType(
        StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> SetPortalAccess(
        Guid id,
        [FromQuery] string status,
        CancellationToken cancellationToken)
    {
        var updated =
            await _patientService.SetPortalAccessAsync(
                id,
                status,
                cancellationToken);

        if (!updated)
        {
            return NotFound();
        }

        return NoContent();
    }

    [HttpDelete("{id:guid}/portal-access")]
    [ProducesResponseType(
        StatusCodes.Status204NoContent)]
    [ProducesResponseType(
        StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DisablePortalAccess(
        Guid id,
        CancellationToken cancellationToken)
    {
        var updated =
            await _patientService.SetPortalAccessAsync(
                id,
                "Disabled",
                cancellationToken);

        if (!updated)
        {
            return NotFound();
        }

        return NoContent();
    }

    [HttpPost("{id:guid}/communication-consent")]
    [ProducesResponseType(
        StatusCodes.Status204NoContent)]
    [ProducesResponseType(
        StatusCodes.Status404NotFound)]
    [ProducesResponseType(
        StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> SetCommunicationConsent(
        Guid id,
        [FromQuery] string status,
        CancellationToken cancellationToken)
    {
        var updated =
            await _patientService.SetCommunicationConsentAsync(
                id,
                status,
                cancellationToken);

        if (!updated)
        {
            return NotFound();
        }

        return NoContent();
    }
}