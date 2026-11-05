using InsureFlow.Application.Payers;
using InsureFlow.Contracts.Payers;
using Microsoft.AspNetCore.Mvc;

namespace InsureFlow.Api.Controllers;

[ApiController]
[Route("api/v1/payers")]
public sealed class PayersController : ControllerBase
{
    private readonly IPayerService _payerService;

    public PayersController(
        IPayerService payerService)
    {
        _payerService = payerService;
    }

    [HttpGet]
    [ProducesResponseType(
        typeof(IReadOnlyList<PayerDto>),
        StatusCodes.Status200OK)]
    public async Task<IActionResult> Get(
        [FromQuery] PayerListRequest request,
        CancellationToken cancellationToken)
    {
        var payers =
            await _payerService.GetPagedAsync(
                request,
                cancellationToken);

        return Ok(payers);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(
        typeof(PayerDto),
        StatusCodes.Status200OK)]
    [ProducesResponseType(
        StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var payer =
            await _payerService.GetByIdAsync(
                id,
                cancellationToken);

        if (payer is null)
        {
            return NotFound();
        }

        return Ok(payer);
    }

    [HttpPost]
    [ProducesResponseType(
        typeof(PayerDto),
        StatusCodes.Status201Created)]
    [ProducesResponseType(
        StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create(
        [FromBody] CreatePayerRequest request,
        CancellationToken cancellationToken)
    {
        var payer =
            await _payerService.CreateAsync(
                request,
                cancellationToken);

        return CreatedAtAction(
            nameof(GetById),
            new { id = payer.Id },
            payer);
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
        [FromBody] UpdatePayerRequest request,
        CancellationToken cancellationToken)
    {
        var updated =
            await _payerService.UpdateAsync(
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
            await _payerService.DeactivateAsync(
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
            await _payerService.ActivateAsync(
                id,
                cancellationToken);

        if (!activated)
        {
            return NotFound();
        }

        return NoContent();
    }
}