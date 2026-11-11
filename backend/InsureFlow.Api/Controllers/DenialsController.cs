
using InsureFlow.Application.Denials;
using InsureFlow.Contracts.Denials;
using Microsoft.AspNetCore.Mvc;

namespace InsureFlow.Api.Controllers;

[ApiController]
[Route("api/v1/denials")]
[Produces("application/json")]
public sealed class DenialsController : ControllerBase
{
    private readonly IDenialService _denialService;

    public DenialsController(IDenialService denialService)
    {
        _denialService = denialService;
    }

    // GET: api/v1/denials
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<DenialResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPaged(
        [FromQuery] string? search,
        [FromQuery] string? status,
        [FromQuery] string? denialCategory,
        [FromQuery] Guid? claimExternalId,
        [FromQuery] DateTime? denialDateFrom,
        [FromQuery] DateTime? denialDateTo,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var result = await _denialService.GetPagedAsync(
            search,
            status,
            denialCategory,
            claimExternalId,
            denialDateFrom,
            denialDateTo,
            page,
            pageSize,
            cancellationToken);

        return Ok(result);
    }

    // GET: api/v1/denials/{id}
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(DenialResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var result = await _denialService.GetByExternalIdAsync(
            id,
            cancellationToken);

        if (result is null)
            return NotFound();

        return Ok(result);
    }

    // POST: api/v1/denials
    [HttpPost]
    [ProducesResponseType(typeof(DenialResponse), StatusCodes.Status201Created)]
    public async Task<IActionResult> Create(
        [FromBody] CreateDenialRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _denialService.CreateAsync(
            request,
            cancellationToken);

        return CreatedAtAction(
            nameof(GetById),
            new { id = result.ExternalId },
            result);
    }

    // PUT: api/v1/denials/{id}
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(DenialResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(
        Guid id,
        [FromBody] UpdateDenialRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _denialService.UpdateAsync(
            id,
            request,
            cancellationToken);

        if (result is null)
            return NotFound();

        return Ok(result);
    }


    [HttpPost("{id:guid}/assign")]
    public async Task<IActionResult> Assign(
        Guid id,
        [FromBody] AssignDenialRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _denialService.AssignAsync(
            id,
            request.UserId,
            cancellationToken);

        if (result is null)
            return NotFound();

        return Ok(result);
    }

    [HttpPost("{id:guid}/unassign")]
    public async Task<IActionResult> Unassign(
        Guid id,
        CancellationToken cancellationToken)
    {
        var result = await _denialService.UnassignAsync(
            id,
            cancellationToken);

        if (result is null)
            return NotFound();

        return Ok(result);
    }

    // GET: api/v1/denials/{id}/activities
    [HttpGet("{id:guid}/activities")]
    [ProducesResponseType(typeof(IReadOnlyList<DenialActivityResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetActivities(
        Guid id,
        CancellationToken cancellationToken)
    {
        var result = await _denialService.GetActivitiesAsync(
            id,
            cancellationToken);

        return Ok(result);
    }

    // POST: api/v1/denials/{id}/activities
    [HttpPost("{id:guid}/activities")]
    [ProducesResponseType(typeof(DenialActivityResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AddActivity(
        Guid id,
        [FromBody] DenialActivityRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _denialService.AddActivityAsync(
            id,
            request,
            cancellationToken);

        if (result is null)
            return NotFound();

        return StatusCode(StatusCodes.Status201Created, result);
    }
    // Denial Attachments

    [HttpGet("{id:guid}/attachments")]
    [ProducesResponseType(typeof(IReadOnlyList<DenialAttachmentResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAttachments(
        Guid id,
        CancellationToken cancellationToken)
    {
        var attachments = await _denialService.GetAttachmentsAsync(
            id,
            cancellationToken);

        return Ok(attachments);
    }

    [HttpPost("{id:guid}/attachments")]
    [ProducesResponseType(typeof(DenialAttachmentResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AddAttachment(
        Guid id,
        [FromBody] CreateDenialAttachmentRequest request,
        CancellationToken cancellationToken)
    {
        var attachment = await _denialService.AddAttachmentAsync(
            id,
            request,
            cancellationToken);

        if (attachment is null)
            return NotFound();

        return Ok(attachment);
    }

    [HttpDelete("{id:guid}/attachments/{attachmentId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteAttachment(
        Guid id,
        Guid attachmentId,
        CancellationToken cancellationToken)
    {
        var deleted = await _denialService.DeleteAttachmentAsync(
            id,
            attachmentId,
            cancellationToken);

        if (!deleted)
            return NotFound();

        return NoContent();
    }

    // Denial Payer Responses

    [HttpGet("{id:guid}/payer-responses")]
    [ProducesResponseType(typeof(IReadOnlyList<DenialPayerResponseResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPayerResponses(
        Guid id,
        CancellationToken cancellationToken)
    {
        var responses = await _denialService.GetPayerResponsesAsync(
            id,
            cancellationToken);

        return Ok(responses);
    }

    [HttpPost("{id:guid}/payer-responses")]
    [ProducesResponseType(typeof(DenialPayerResponseResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AddPayerResponse(
        Guid id,
        [FromBody] CreateDenialPayerResponseRequest request,
        CancellationToken cancellationToken)
    {
        var response = await _denialService.AddPayerResponseAsync(
            id,
            request,
            cancellationToken);

        if (response is null)
            return NotFound();

        return Ok(response);
    }
}
