using InsureFlow.Application.Providers;
using InsureFlow.Contracts.Providers;
using Microsoft.AspNetCore.Mvc;

namespace InsureFlow.Api.Controllers;

[ApiController]
[Route("api/v1/providers")]
public sealed class ProvidersController : ControllerBase
{
    private readonly IProviderService _providerService;

    public ProvidersController(
        IProviderService providerService)
    {
        _providerService = providerService;
    }

    [HttpGet]
    [ProducesResponseType(
        typeof(IReadOnlyList<ProviderDto>),
        StatusCodes.Status200OK)]
    public async Task<IActionResult> Get(
        [FromQuery] ProviderListRequest request,
        CancellationToken cancellationToken)
    {
        var providers =
            await _providerService.GetPagedAsync(
                request,
                cancellationToken);

        return Ok(providers);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(
        typeof(ProviderDto),
        StatusCodes.Status200OK)]
    [ProducesResponseType(
        StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var provider =
            await _providerService.GetByIdAsync(
                id,
                cancellationToken);

        if (provider is null)
        {
            return NotFound();
        }

        return Ok(provider);
    }

    [HttpPost]
    [ProducesResponseType(
        typeof(ProviderDto),
        StatusCodes.Status201Created)]
    [ProducesResponseType(
        StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create(
        [FromBody] CreateProviderRequest request,
        CancellationToken cancellationToken)
    {
        var provider =
            await _providerService.CreateAsync(
                request,
                cancellationToken);

        return CreatedAtAction(
            nameof(GetById),
            new { id = provider.Id },
            provider);
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
        [FromBody] UpdateProviderRequest request,
        CancellationToken cancellationToken)
    {
        var updated =
            await _providerService.UpdateAsync(
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
            await _providerService.DeactivateAsync(
                id,
                cancellationToken);

        if (!deactivated)
        {
            return NotFound();
        }

        return NoContent();
    }
}