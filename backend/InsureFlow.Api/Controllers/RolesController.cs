
using InsureFlow.Application.Roles;
using InsureFlow.Contracts.Roles;
using Microsoft.AspNetCore.Mvc;

namespace InsureFlow.Api.Controllers;

[ApiController]
[Route("api/v1/roles")]
[Produces("application/json")]
public sealed class RolesController : ControllerBase
{
    private readonly IRoleService _roleService;

    public RolesController(IRoleService roleService)
    {
        _roleService = roleService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<RoleResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(
        CancellationToken cancellationToken)
    {
        return Ok(await _roleService.GetAllAsync(cancellationToken));
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(RoleResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var result = await _roleService.GetByIdAsync(id, cancellationToken);

        return result is null ? NotFound() : Ok(result);
    }

    [HttpPost]
    [ProducesResponseType(typeof(RoleResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Create(
        [FromBody] CreateRoleRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await _roleService.CreateAsync(
                request,
                cancellationToken);

            return CreatedAtAction(
                nameof(GetById),
                new { id = result.ExternalId },
                result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(RoleResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Update(
        Guid id,
        [FromBody] UpdateRoleRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await _roleService.UpdateAsync(
                id,
                request,
                cancellationToken);

            return result is null ? NotFound() : Ok(result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(
        Guid id,
        CancellationToken cancellationToken)
    {
        var deleted = await _roleService.DeleteAsync(id, cancellationToken);

        return deleted ? NoContent() : NotFound();
    }

    [HttpGet("{id:guid}/permissions")]
    [ProducesResponseType(typeof(IReadOnlyList<PermissionResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetPermissions(
        Guid id,
        CancellationToken cancellationToken)
    {
        var result = await _roleService.GetPermissionsAsync(
            id,
            cancellationToken);

        return result is null ? NotFound() : Ok(result);
    }

    [HttpPut("{id:guid}/permissions")]
    [ProducesResponseType(typeof(IReadOnlyList<PermissionResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdatePermissions(
        Guid id,
        [FromBody] UpdateRolePermissionsRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await _roleService.UpdatePermissionsAsync(
                id,
                request,
                cancellationToken);

            return result is null ? NotFound() : Ok(result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}
