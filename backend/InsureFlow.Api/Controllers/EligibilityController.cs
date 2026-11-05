
using InsureFlow.Application.Eligibility;
using InsureFlow.Contracts.Eligibility;
using Microsoft.AspNetCore.Mvc;

namespace InsureFlow.Api.Controllers;

[ApiController]
[Route("api/v1/eligibility")]
[Produces("application/json")]
public sealed class EligibilityController(
    IEligibilityService eligibilityService) : ControllerBase
{
    private readonly IEligibilityService _eligibilityService =
        eligibilityService;

    /// <summary>
    /// Creates an eligibility verification request.
    /// </summary>
    [HttpPost("requests")]
    [ProducesResponseType(
        typeof(EligibilityRequestDto),
        StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<EligibilityRequestDto>> Create(
        [FromBody] CreateEligibilityRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _eligibilityService.CreateAsync(
            request,
            cancellationToken);

        return CreatedAtAction(
            nameof(GetById),
            new { id = result.Id },
            result);
    }

    /// <summary>
    /// Gets a paginated list of eligibility requests.
    /// </summary>
    [HttpGet("requests")]
    [ProducesResponseType(
        typeof(EligibilityListResponse),
        StatusCodes.Status200OK)]
    public async Task<ActionResult<EligibilityListResponse>> GetPaged(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? status = null,
        [FromQuery] long? patientId = null,
        [FromQuery] long? patientInsuranceId = null,
        [FromQuery] long? payerId = null,
        CancellationToken cancellationToken = default)
    {
        if (pageNumber < 1 || pageSize < 1 || pageSize > 100)
        {
            return BadRequest(new
            {
                message =
                    "Page number must be at least 1, and page size must be between 1 and 100."
            });
        }

        var result = await _eligibilityService.GetPagedAsync(
            pageNumber,
            pageSize,
            status,
            patientId,
            patientInsuranceId,
            payerId,
            cancellationToken);

        return Ok(result);
    }

    /// <summary>
    /// Gets an eligibility request by its internal database ID.
    /// </summary>
    [HttpGet("requests/{id:long}")]
    [ProducesResponseType(
        typeof(EligibilityRequestDto),
        StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<EligibilityRequestDto>> GetById(
        long id,
        [FromQuery] bool includeDetails = true,
        CancellationToken cancellationToken = default)
    {
        var result = await _eligibilityService.GetByIdAsync(
            id,
            includeDetails,
            cancellationToken);

        if (result is null)
        {
            return NotFound(new
            {
                message = $"Eligibility request with ID {id} was not found."
            });
        }

        return Ok(result);
    }

    /// <summary>
    /// Gets an eligibility request by its external GUID.
    /// </summary>
    [HttpGet("requests/by-external-id/{externalId:guid}")]
    [ProducesResponseType(
        typeof(EligibilityRequestDto),
        StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<EligibilityRequestDto>> GetByExternalId(
        Guid externalId,
        [FromQuery] bool includeDetails = true,
        CancellationToken cancellationToken = default)
    {
        var result =
            await _eligibilityService.GetByExternalIdAsync(
                externalId,
                includeDetails,
                cancellationToken);

        if (result is null)
        {
            return NotFound(new
            {
                message =
                    $"Eligibility request '{externalId}' was not found."
            });
        }

        return Ok(result);
    }

    /// <summary>
    /// Marks an eligibility request as processing.
    /// </summary>
    [HttpPost("requests/{id:long}/processing")]
    [ProducesResponseType(
        typeof(EligibilityRequestDto),
        StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<EligibilityRequestDto>> MarkProcessing(
        long id,
        CancellationToken cancellationToken)
    {
        var result = await _eligibilityService.MarkProcessingAsync(
            id,
            cancellationToken);

        return Ok(result);
    }

    /// <summary>
    /// Records the payer's eligibility response.
    /// </summary>
    [HttpPost("requests/{id:long}/response")]
    [ProducesResponseType(
        typeof(EligibilityRequestDto),
        StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<EligibilityRequestDto>> RecordResponse(
        long id,
        [FromBody] RecordEligibilityResponseRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _eligibilityService.RecordResponseAsync(
            id,
            request,
            cancellationToken);

        return Ok(result);
    }

    /// <summary>
    /// Marks an eligibility request as failed.
    /// </summary>
    [HttpPost("requests/{id:long}/fail")]
    [ProducesResponseType(
        typeof(EligibilityRequestDto),
        StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<EligibilityRequestDto>> MarkFailed(
        long id,
        [FromBody] MarkEligibilityFailedRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _eligibilityService.MarkFailedAsync(
            id,
            request.ErrorCode,
            request.ErrorMessage,
            cancellationToken);

        return Ok(result);
    }

    /// <summary>
    /// Cancels an eligibility request.
    /// </summary>
    [HttpPost("requests/{id:long}/cancel")]
    [ProducesResponseType(
        typeof(EligibilityRequestDto),
        StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<EligibilityRequestDto>> Cancel(
        long id,
        CancellationToken cancellationToken)
    {
        var result = await _eligibilityService.CancelAsync(
            id,
            cancellationToken);

        return Ok(result);
    }

    /// <summary>
    /// Gets the status history of an eligibility request.
    /// </summary>
    [HttpGet("requests/{id:long}/history")]
    [ProducesResponseType(
        typeof(IReadOnlyList<EligibilityHistoryDto>),
        StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<EligibilityHistoryDto>>>
        GetHistory(
            long id,
            CancellationToken cancellationToken)
    {
        var result = await _eligibilityService.GetHistoryAsync(
            id,
            cancellationToken);

        return Ok(result);
    }
}

/// <summary>
/// Request body used to mark an eligibility request as failed.
/// </summary>
public sealed class MarkEligibilityFailedRequest
{
    public string? ErrorCode { get; set; }

    public string? ErrorMessage { get; set; }
}