
using InsureFlow.Application.Claims;
using InsureFlow.Contracts.Claims;
using Microsoft.AspNetCore.Mvc;

namespace InsureFlow.Api.Controllers;

[ApiController]
[Route("api/v1/claims")]
[Produces("application/json")]
public sealed class ClaimsController : ControllerBase
{
    private readonly IClaimService _claimService;

    public ClaimsController(IClaimService claimService)
    {
        _claimService = claimService;
    }

    // GET /api/v1/claims
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<ClaimDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<ClaimDto>>> GetClaims(
        [FromQuery] ClaimListRequest request,
        CancellationToken cancellationToken)
    {
        var claims = await _claimService.GetPagedAsync(request, cancellationToken);
        return Ok(claims);
    }

    // GET /api/v1/claims/{id}
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ClaimDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ClaimDto>> GetClaimById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var claim = await _claimService.GetByIdAsync(id, cancellationToken);

        if (claim is null)
            return NotFound();

        return Ok(claim);
    }

    // POST /api/v1/claims
    [HttpPost]
    [ProducesResponseType(typeof(ClaimDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ClaimDto>> CreateClaim(
        [FromBody] CreateClaimRequest request,
        CancellationToken cancellationToken)
    {
        var claim = await _claimService.CreateAsync(request, cancellationToken);

        return CreatedAtAction(
            nameof(GetClaimById),
            new { id = claim.Id },
            claim);
    }

    // PUT /api/v1/claims/{id}
    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateClaim(
        Guid id,
        [FromBody] UpdateClaimRequest request,
        CancellationToken cancellationToken)
    {
        var updated = await _claimService.UpdateAsync(id, request, cancellationToken);

        if (!updated)
            return NotFound();

        return NoContent();
    }

    // POST /api/v1/claims/{id}/validate
    [HttpPost("{id:guid}/validate")]
    [ProducesResponseType(typeof(ClaimValidationDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ClaimValidationDto>> ValidateClaim(
        Guid id,
        CancellationToken cancellationToken)
    {
        var result = await _claimService.ValidateAsync(id, cancellationToken);
        return Ok(result);
    }

    // GET /api/v1/claims/{id}/validation
    [HttpGet("{id:guid}/validation")]
    [ProducesResponseType(typeof(ClaimValidationDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ClaimValidationDto>> GetClaimValidation(
        Guid id,
        CancellationToken cancellationToken)
    {
        var result = await _claimService.GetValidationAsync(id, cancellationToken);

        if (result is null)
            return NotFound();

        return Ok(result);
    }

    // POST /api/v1/claims/{id}/submit
    [HttpPost("{id:guid}/submit")]
    [ProducesResponseType(typeof(ClaimDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ClaimDto>> SubmitClaim(
        Guid id,
        CancellationToken cancellationToken)
    {
        var claim = await _claimService.SubmitAsync(id, cancellationToken);
        return Ok(claim);
    }

    // POST /api/v1/claims/{id}/resubmit
    [HttpPost("{id:guid}/resubmit")]
    [ProducesResponseType(typeof(ClaimDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ClaimDto>> ResubmitClaim(
        Guid id,
        CancellationToken cancellationToken)
    {
        var claim = await _claimService.ResubmitAsync(id, cancellationToken);

        if (claim is null)
            return NotFound();

        return Ok(claim);
    }

    // GET /api/v1/claims/{id}/rejections
    [HttpGet("{id:guid}/rejections")]
    [ProducesResponseType(typeof(IReadOnlyList<ClaimRejectionDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IReadOnlyList<ClaimRejectionDto>>> GetClaimRejections(
        Guid id,
        CancellationToken cancellationToken)
    {
        var rejections = await _claimService.GetRejectionsAsync(id, cancellationToken);

        if (rejections is null)
            return NotFound();

        return Ok(rejections);
    }

    // GET /api/v1/claims/{id}/response
    [HttpGet("{id:guid}/response")]
    [ProducesResponseType(typeof(ClaimSubmissionResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ClaimSubmissionResponseDto>> GetClaimResponse(
        Guid id,
        CancellationToken cancellationToken)
    {
        var response = await _claimService.GetSubmissionResponseAsync(id, cancellationToken);

        if (response is null)
            return NotFound();

        return Ok(response);
    }

    // GET /api/v1/claims/{id}/history
    [HttpGet("{id:guid}/history")]
    [ProducesResponseType(typeof(IReadOnlyList<ClaimHistoryDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IReadOnlyList<ClaimHistoryDto>>> GetClaimHistory(
        Guid id,
        CancellationToken cancellationToken)
    {
        var history = await _claimService.GetHistoryAsync(id, cancellationToken);

        if (history is null)
            return NotFound();

        return Ok(history);
    }

    // GET /api/v1/claims/{claimId}/lines
    [HttpGet("{claimId:guid}/lines")]
    [ProducesResponseType(typeof(IReadOnlyList<ClaimLineDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IReadOnlyList<ClaimLineDto>>> GetClaimLines(
        Guid claimId,
        CancellationToken cancellationToken)
    {
        var lines = await _claimService.GetLinesAsync(claimId, cancellationToken);

        if (lines is null)
            return NotFound();

        return Ok(lines);
    }

    // POST /api/v1/claims/{claimId}/lines
    [HttpPost("{claimId:guid}/lines")]
    [ProducesResponseType(typeof(ClaimLineDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ClaimLineDto>> AddClaimLine(
        Guid claimId,
        [FromBody] ClaimLineInput request,
        CancellationToken cancellationToken)
    {
        var line = await _claimService.AddLineAsync(claimId, request, cancellationToken);

        if (line is null)
            return NotFound();

        return CreatedAtAction(nameof(GetClaimLines), new { claimId }, line);
    }

    // PUT /api/v1/claims/{claimId}/lines/{lineId}
    [HttpPut("{claimId:guid}/lines/{lineId:guid}")]
    [ProducesResponseType(typeof(ClaimLineDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ClaimLineDto>> UpdateClaimLine(
        Guid claimId,
        Guid lineId,
        [FromBody] ClaimLineInput request,
        CancellationToken cancellationToken)
    {
        var line = await _claimService.UpdateLineAsync(
            claimId, lineId, request, cancellationToken);

        if (line is null)
            return NotFound();

        return Ok(line);
    }

    // DELETE /api/v1/claims/{claimId}/lines/{lineId}
    [HttpDelete("{claimId:guid}/lines/{lineId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteClaimLine(
        Guid claimId,
        Guid lineId,
        CancellationToken cancellationToken)
    {
        var deleted = await _claimService.DeleteLineAsync(
            claimId, lineId, cancellationToken);

        if (!deleted)
            return NotFound();

        return NoContent();
    }

    // GET /api/v1/claims/{id}/diagnoses
    [HttpGet("{id:guid}/diagnoses")]
    [ProducesResponseType(typeof(IReadOnlyList<ClaimDiagnosisDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IReadOnlyList<ClaimDiagnosisDto>>> GetClaimDiagnoses(
        Guid id,
        CancellationToken cancellationToken)
    {
        var diagnoses = await _claimService.GetDiagnosesAsync(id, cancellationToken);

        if (diagnoses is null)
            return NotFound();

        return Ok(diagnoses);
    }

    // POST /api/v1/claims/{id}/diagnoses
    [HttpPost("{id:guid}/diagnoses")]
    [ProducesResponseType(typeof(ClaimDiagnosisDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ClaimDiagnosisDto>> AddClaimDiagnosis(
        Guid id,
        [FromBody] ClaimDiagnosisInput request,
        CancellationToken cancellationToken)
    {
        var diagnosis = await _claimService.AddDiagnosisAsync(
            id, request, cancellationToken);

        if (diagnosis is null)
            return NotFound();

        return CreatedAtAction(
            nameof(GetClaimDiagnoses),
            new { id },
            diagnosis);
    }

    // DELETE /api/v1/claims/{id}/diagnoses/{diagnosisId}
    [HttpDelete("{id:guid}/diagnoses/{diagnosisId:long}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteClaimDiagnosis(
        Guid id,
        long diagnosisId,
        CancellationToken cancellationToken)
    {
        var deleted = await _claimService.DeleteDiagnosisAsync(
            id, diagnosisId, cancellationToken);

        if (!deleted)
            return NotFound();

        return NoContent();
    }

    // GET /api/v1/claims/{id}/attachments
    [HttpGet("{id:guid}/attachments")]
    [ProducesResponseType(typeof(IReadOnlyList<ClaimAttachmentDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IReadOnlyList<ClaimAttachmentDto>>> GetClaimAttachments(
        Guid id,
        CancellationToken cancellationToken)
    {
        var attachments = await _claimService.GetAttachmentsAsync(id, cancellationToken);

        if (attachments is null)
            return NotFound();

        return Ok(attachments);
    }

    // POST /api/v1/claims/{id}/attachments
    [HttpPost("{id:guid}/attachments")]
    [ProducesResponseType(typeof(ClaimAttachmentDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ClaimAttachmentDto>> AddClaimAttachment(
        Guid id,
        [FromBody] ClaimAttachmentInput request,
        CancellationToken cancellationToken)
    {
        var attachment = await _claimService.AddAttachmentAsync(
            id, request, cancellationToken);

        if (attachment is null)
            return NotFound();

        return CreatedAtAction(
            nameof(GetClaimAttachments),
            new { id },
            attachment);
    }

    // DELETE /api/v1/claims/{id}/attachments/{attachmentId}
    [HttpDelete("{id:guid}/attachments/{attachmentId:long}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteClaimAttachment(
        Guid id,
        long attachmentId,
        CancellationToken cancellationToken)
    {
        var deleted = await _claimService.DeleteAttachmentAsync(
            id, attachmentId, cancellationToken);

        if (!deleted)
            return NotFound();

        return NoContent();
    }


    // =========================================================
    // CLAIM STATUS
    // =========================================================

    // GET: /api/v1/claims/status
    [HttpGet("status")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPagedStatuses(
        [FromQuery] ClaimListRequest request,
        CancellationToken cancellationToken)
    {
        var statuses = await _claimService.GetPagedStatusesAsync(
            request,
            cancellationToken);

        return Ok(statuses);
    }

    // GET: /api/v1/claims/{id}/status
    [HttpGet("{id:guid}/status")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetStatus(
        Guid id,
        CancellationToken cancellationToken)
    {
        var status = await _claimService.GetStatusAsync(
            id,
            cancellationToken);

        if (status is null)
            return NotFound();

        return Ok(status);
    }

    // GET: /api/v1/claims/{id}/status-history
    [HttpGet("{id:guid}/status-history")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetStatusHistory(
        Guid id,
        CancellationToken cancellationToken)
    {
        var history = await _claimService.GetStatusHistoryAsync(
            id,
            cancellationToken);

        if (history is null)
            return NotFound();

        return Ok(history);
    }

    // GET: /api/v1/claims/{id}/current-status
    [HttpGet("{id:guid}/current-status")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetCurrentStatus(
        Guid id,
        CancellationToken cancellationToken)
    {
        var status = await _claimService.GetStatusAsync(
            id,
            cancellationToken);

        if (status is null)
            return NotFound();

        return Ok(status);
    }


    // =========================================================
    // CLAIM STATUS CHECK AND REFRESH
    // =========================================================

    // POST: /api/v1/claims/status/check
    [HttpPost("status/check")]
    [ProducesResponseType(StatusCodes.Status501NotImplemented)]
    public IActionResult CheckClaimStatus(
        [FromBody] ClaimStatusCheckRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (request.ClaimId == Guid.Empty)
            return BadRequest("A valid ClaimId is required.");

        return Problem(
            statusCode: StatusCodes.Status501NotImplemented,
            title: "Claim status checking is not configured.",
            detail:
                "Connect a payer or clearinghouse integration before requesting an external claim status check.");
    }

    // POST: /api/v1/claims/{id}/refresh
    [HttpPost("{id:guid}/refresh")]
    [ProducesResponseType(StatusCodes.Status501NotImplemented)]
    public IActionResult RefreshClaimStatus(Guid id)
    {
        if (id == Guid.Empty)
            return BadRequest("A valid claim ID is required.");

        return Problem(
            statusCode: StatusCodes.Status501NotImplemented,
            title: "Claim status refresh is not configured.",
            detail:
                "Connect a payer or clearinghouse integration before refreshing the claim status.");
    }


}
