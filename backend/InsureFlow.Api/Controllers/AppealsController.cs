
using InsureFlow.Application.Appeals;
using InsureFlow.Contracts.Appeals;
using Microsoft.AspNetCore.Mvc;

namespace InsureFlow.Api.Controllers;

[ApiController]
[Route("api/v1/appeals")]
[Produces("application/json")]
public sealed class AppealsController : ControllerBase
{
    private readonly IAppealService _appealService;

    public AppealsController(IAppealService appealService)
    {
        _appealService = appealService;
    }

    // GET: api/v1/appeals
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<AppealResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<AppealResponse>>> GetPaged(
        [FromQuery] string? search,
        [FromQuery] string? status,
        [FromQuery] Guid? denialExternalId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var result = await _appealService.GetPagedAsync(
            search,
            status,
            denialExternalId,
            page,
            pageSize,
            cancellationToken);

        return Ok(result);
    }

    // GET: api/v1/appeals/external-review
    [HttpGet("external-review")]
    [ProducesResponseType(typeof(IReadOnlyList<AppealExternalReviewResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<AppealExternalReviewResponse>>> GetExternalReviews(
        [FromQuery] string? status,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var result = await _appealService.GetExternalReviewsAsync(
            status,
            page,
            pageSize,
            cancellationToken);

        return Ok(result);
    }

    // GET: api/v1/appeals/{id}
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(AppealResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AppealResponse>> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var result = await _appealService.GetByExternalIdAsync(
            id,
            cancellationToken);

        if (result is null)
            return NotFound();

        return Ok(result);
    }

    // POST: api/v1/appeals
    [HttpPost]
    [ProducesResponseType(typeof(AppealResponse), StatusCodes.Status201Created)]
    public async Task<ActionResult<AppealResponse>> Create(
        [FromBody] CreateAppealRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _appealService.CreateAsync(
            request,
            cancellationToken);

        return CreatedAtAction(
            nameof(GetById),
            new { id = result.ExternalId },
            result);
    }

    // PUT: api/v1/appeals/{id}
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(AppealResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AppealResponse>> Update(
        Guid id,
        [FromBody] UpdateAppealRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _appealService.UpdateAsync(
            id,
            request,
            cancellationToken);

        if (result is null)
            return NotFound();

        return Ok(result);
    }

    // POST: api/v1/appeals/{id}/submit
    [HttpPost("{id:guid}/submit")]
    [ProducesResponseType(typeof(AppealResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AppealResponse>> Submit(
        Guid id,
        CancellationToken cancellationToken)
    {
        var result = await _appealService.SubmitAsync(
            id,
            cancellationToken);

        if (result is null)
            return NotFound();

        return Ok(result);
    }

    // POST: api/v1/appeals/{id}/withdraw
    [HttpPost("{id:guid}/withdraw")]
    [ProducesResponseType(typeof(AppealResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AppealResponse>> Withdraw(
        Guid id,
        CancellationToken cancellationToken)
    {
        var result = await _appealService.WithdrawAsync(
            id,
            cancellationToken);

        if (result is null)
            return NotFound();

        return Ok(result);
    }

    // POST: api/v1/appeals/{id}/decision
    [HttpPost("{id:guid}/decision")]
    [ProducesResponseType(typeof(AppealResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AppealResponse>> RecordDecision(
        Guid id,
        [FromBody] RecordAppealDecisionRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _appealService.RecordDecisionAsync(
            id,
            request,
            cancellationToken);

        if (result is null)
            return NotFound();

        return Ok(result);
    }

    // GET: api/v1/appeals/{id}/documentation
    [HttpGet("{id:guid}/documentation")]
    [ProducesResponseType(typeof(IReadOnlyList<AppealDocumentationResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IReadOnlyList<AppealDocumentationResponse>>> GetDocumentation(
        Guid id,
        CancellationToken cancellationToken)
    {
        if (await _appealService.GetByExternalIdAsync(id, cancellationToken) is null)
            return NotFound();

        var result = await _appealService.GetDocumentationAsync(
            id,
            cancellationToken);

        return Ok(result);
    }

    // POST: api/v1/appeals/{id}/documentation
    [HttpPost("{id:guid}/documentation")]
    [ProducesResponseType(typeof(AppealDocumentationResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AppealDocumentationResponse>> AddDocumentation(
        Guid id,
        [FromBody] AppealDocumentationRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _appealService.AddDocumentationAsync(
            id,
            request,
            cancellationToken);

        if (result is null)
            return NotFound();

        return StatusCode(StatusCodes.Status201Created, result);
    }

    // GET: api/v1/appeals/{id}/attachments
    [HttpGet("{id:guid}/attachments")]
    [ProducesResponseType(typeof(IReadOnlyList<AppealAttachmentResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IReadOnlyList<AppealAttachmentResponse>>> GetAttachments(
        Guid id,
        CancellationToken cancellationToken)
    {
        if (await _appealService.GetByExternalIdAsync(id, cancellationToken) is null)
            return NotFound();

        var result = await _appealService.GetAttachmentsAsync(
            id,
            cancellationToken);

        return Ok(result);
    }

    // POST: api/v1/appeals/{id}/attachments
    [HttpPost("{id:guid}/attachments")]
    [ProducesResponseType(typeof(AppealAttachmentResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AppealAttachmentResponse>> AddAttachment(
        Guid id,
        [FromBody] AppealAttachmentRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _appealService.AddAttachmentAsync(
            id,
            request,
            cancellationToken);

        if (result is null)
            return NotFound();

        return StatusCode(StatusCodes.Status201Created, result);
    }

    // DELETE: api/v1/appeals/{id}/attachments/{attachmentId}
    [HttpDelete("{id:guid}/attachments/{attachmentId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteAttachment(
        Guid id,
        Guid attachmentId,
        CancellationToken cancellationToken)
    {
        var deleted = await _appealService.DeleteAttachmentAsync(
            id,
            attachmentId,
            cancellationToken);

        if (!deleted)
            return NotFound();

        return NoContent();
    }

    // POST: api/v1/appeals/{id}/external-review
    [HttpPost("{id:guid}/external-review")]
    [ProducesResponseType(typeof(AppealExternalReviewResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AppealExternalReviewResponse>> CreateExternalReview(
        Guid id,
        [FromBody] AppealExternalReviewRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _appealService.CreateExternalReviewAsync(
            id,
            request,
            cancellationToken);

        if (result is null)
            return NotFound();

        return StatusCode(StatusCodes.Status201Created, result);
    }

    // GET: api/v1/appeals/{id}/history
    [HttpGet("{id:guid}/history")]
    [ProducesResponseType(typeof(IReadOnlyList<AppealHistoryResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IReadOnlyList<AppealHistoryResponse>>> GetHistory(
        Guid id,
        CancellationToken cancellationToken)
    {
        if (await _appealService.GetByExternalIdAsync(id, cancellationToken) is null)
            return NotFound();

        var result = await _appealService.GetHistoryAsync(
            id,
            cancellationToken);

        return Ok(result);
    }

    // GET: api/v1/appeals/{id}/notifications
    [HttpGet("{id:guid}/notifications")]
    [ProducesResponseType(typeof(IReadOnlyList<AppealNotificationResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IReadOnlyList<AppealNotificationResponse>>> GetNotifications(
        Guid id,
        CancellationToken cancellationToken)
    {
        if (await _appealService.GetByExternalIdAsync(id, cancellationToken) is null)
            return NotFound();

        var result = await _appealService.GetNotificationsAsync(
            id,
            cancellationToken);

        return Ok(result);
    }

    // POST: api/v1/appeals/{id}/notifications/read
    [HttpPost("{id:guid}/notifications/read")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> MarkNotificationsRead(
        Guid id,
        [FromBody] MarkAppealNotificationsReadRequest request,
        CancellationToken cancellationToken)
    {
        var markedCount = await _appealService.MarkNotificationsReadAsync(
            id,
            request,
            cancellationToken);

        if (markedCount < 0)
            return NotFound();

        return Ok(new
        {
            appealExternalId = id,
            markedAsRead = markedCount
        });
    }
}
