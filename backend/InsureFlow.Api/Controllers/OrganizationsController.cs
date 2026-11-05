using InsureFlow.Application.Organizations;
using InsureFlow.Contracts.Organizations;
using Microsoft.AspNetCore.Mvc;

namespace InsureFlow.Api.Controllers;

[ApiController]
[Route("api/v1/organizations")]
public sealed class OrganizationsController : ControllerBase
{
    private readonly IOrganizationService _organizationService;

    public OrganizationsController(
        IOrganizationService organizationService)
    {
        _organizationService = organizationService;
    }

    [HttpGet]
    [ProducesResponseType(
        typeof(IReadOnlyList<OrganizationDto>),
        StatusCodes.Status200OK)]
    public async Task<IActionResult> Get(
        [FromQuery] OrganizationListRequest request,
        CancellationToken cancellationToken)
    {
        var organizations =
            await _organizationService.GetPagedAsync(
                request,
                cancellationToken);

        return Ok(organizations);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(
        typeof(OrganizationDto),
        StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var organization =
            await _organizationService.GetByIdAsync(
                id,
                cancellationToken);

        if (organization is null)
        {
            return NotFound();
        }

        return Ok(organization);
    }

    [HttpPost]
    [ProducesResponseType(
        typeof(OrganizationDto),
        StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create(
        [FromBody] CreateOrganizationRequest request,
        CancellationToken cancellationToken)
    {
        var organization =
            await _organizationService.CreateAsync(
                request,
                cancellationToken);

        return CreatedAtAction(
            nameof(GetById),
            new { id = organization.Id },
            organization);
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Update(
        Guid id,
        [FromBody] UpdateOrganizationRequest request,
        CancellationToken cancellationToken)
    {
        var updated =
            await _organizationService.UpdateAsync(
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
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(
        Guid id,
        CancellationToken cancellationToken)
    {
        var deactivated =
            await _organizationService.DeactivateAsync(
                id,
                cancellationToken);

        if (!deactivated)
        {
            return NotFound();
        }

        return NoContent();
    }
}