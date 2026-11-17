
using InsureFlow.Application.Payments;
using InsureFlow.Contracts.Payments;
using Microsoft.AspNetCore.Mvc;

namespace InsureFlow.Api.Controllers;

[ApiController]
[Route("api/v1/payments")]
[Produces("application/json")]
public sealed class PaymentsController : ControllerBase
{
    private readonly IPaymentService _paymentService;

    public PaymentsController(IPaymentService paymentService)
    {
        _paymentService = paymentService;
    }

    // GET /api/v1/payments
    [HttpGet]
    [ProducesResponseType(typeof(PaymentListResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<PaymentListResponse>> GetPayments(
        [FromQuery] PaymentQueryRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _paymentService.GetPaymentsAsync(
            request, cancellationToken);

        return Ok(result);
    }

    // GET /api/v1/payments/{id}
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(PaymentResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PaymentResponse>> GetPayment(
        Guid id,
        CancellationToken cancellationToken)
    {
        var result = await _paymentService.GetPaymentAsync(
            id, cancellationToken);

        if (result is null)
            return NotFound();

        return Ok(result);
    }

    // POST /api/v1/payments
    [HttpPost]
    [ProducesResponseType(typeof(PaymentResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PaymentResponse>> CreatePayment(
        [FromBody] CreatePaymentRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _paymentService.CreatePaymentAsync(
            request, cancellationToken);

        return CreatedAtAction(
            nameof(GetPayment),
            new { id = result.Id },
            result);
    }

    // PUT /api/v1/payments/{id}
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(PaymentResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PaymentResponse>> UpdatePayment(
        Guid id,
        [FromBody] UpdatePaymentRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _paymentService.UpdatePaymentAsync(
            id, request, cancellationToken);

        if (result is null)
            return NotFound();

        return Ok(result);
    }

    // POST /api/v1/payments/{id}/post
    [HttpPost("{id:guid}/post")]
    [ProducesResponseType(typeof(PaymentResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PaymentResponse>> PostPayment(
        Guid id,
        CancellationToken cancellationToken)
    {
        var result = await _paymentService.PostPaymentAsync(
            id, cancellationToken);

        if (result is null)
            return NotFound();

        return Ok(result);
    }

    // POST /api/v1/payments/{id}/reverse
    [HttpPost("{id:guid}/reverse")]
    [ProducesResponseType(typeof(PaymentResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PaymentResponse>> ReversePayment(
        Guid id,
        CancellationToken cancellationToken)
    {
        var result = await _paymentService.ReversePaymentAsync(
            id, cancellationToken);

        if (result is null)
            return NotFound();

        return Ok(result);
    }


    // GET /api/v1/payments/{paymentId}/allocations
    [HttpGet("{paymentId:guid}/allocations")]
    [ProducesResponseType(typeof(IReadOnlyList<PaymentAllocationResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IReadOnlyList<PaymentAllocationResponse>>> GetAllocations(
        Guid paymentId,
        CancellationToken cancellationToken)
    {
        var result = await _paymentService.GetAllocationsAsync(
            paymentId, cancellationToken);

        if (result is null)
            return NotFound();

        return Ok(result);
    }

    // POST /api/v1/payments/{paymentId}/allocations
    [HttpPost("{paymentId:guid}/allocations")]
    [ProducesResponseType(typeof(PaymentAllocationResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PaymentAllocationResponse>> CreateAllocation(
        Guid paymentId,
        [FromBody] CreatePaymentAllocationRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _paymentService.CreateAllocationAsync(
            paymentId, request, cancellationToken);

        if (result is null)
            return NotFound();

        return CreatedAtAction(
            nameof(GetAllocations),
            new { paymentId },
            result);
    }

    // PUT /api/v1/payments/{paymentId}/allocations/{allocationId}
    [HttpPut("{paymentId:guid}/allocations/{allocationId:guid}")]
    [ProducesResponseType(typeof(PaymentAllocationResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PaymentAllocationResponse>> UpdateAllocation(
        Guid paymentId,
        Guid allocationId,
        [FromBody] UpdatePaymentAllocationRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _paymentService.UpdateAllocationAsync(
            paymentId, allocationId, request, cancellationToken);

        if (result is null)
            return NotFound();

        return Ok(result);
    }

    // GET /api/v1/payments/era
    [HttpGet("era")]
    [ProducesResponseType(typeof(EraListResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<EraListResponse>> GetEraRecords(
        [FromQuery] EraQueryRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _paymentService.GetEraRecordsAsync(
            request, cancellationToken);

        return Ok(result);
    }

    // GET /api/v1/payments/era/{id}
    [HttpGet("era/{id:guid}")]
    [ProducesResponseType(typeof(EraResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<EraResponse>> GetEraRecord(
        Guid id,
        CancellationToken cancellationToken)
    {
        var result = await _paymentService.GetEraRecordAsync(
            id, cancellationToken);

        if (result is null)
            return NotFound();

        return Ok(result);
    }

    // POST /api/v1/payments/era/import
    [HttpPost("era/import")]
    [ProducesResponseType(typeof(EraResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<EraResponse>> ImportEra(
        [FromBody] ImportEraRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _paymentService.ImportEraAsync(
            request, cancellationToken);

        return CreatedAtAction(
            nameof(GetEraRecord),
            new { id = result.Id },
            result);
    }


    // GET /api/v1/payments/eob
    [HttpGet("eob")]
    [ProducesResponseType(typeof(EobListResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<EobListResponse>> GetEobRecords(
        [FromQuery] EobQueryRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _paymentService.GetEobRecordsAsync(
            request, cancellationToken);

        return Ok(result);
    }

    // GET /api/v1/payments/eob/{id}
    [HttpGet("eob/{id:guid}")]
    [ProducesResponseType(typeof(EobResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<EobResponse>> GetEobRecord(
        Guid id,
        CancellationToken cancellationToken)
    {
        var result = await _paymentService.GetEobRecordAsync(
            id, cancellationToken);

        if (result is null)
            return NotFound();

        return Ok(result);
    }

    // POST /api/v1/payments/eob/upload
    [HttpPost("eob/upload")]
    [ProducesResponseType(typeof(EobResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<EobResponse>> UploadEob(
        [FromBody] UploadEobRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _paymentService.UploadEobAsync(
            request, cancellationToken);

        return CreatedAtAction(
            nameof(GetEobRecord),
            new { id = result.Id },
            result);
    }

    // GET /api/v1/payments/adjustments
    [HttpGet("adjustments")]
    [ProducesResponseType(typeof(IReadOnlyList<PaymentAdjustmentResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<PaymentAdjustmentResponse>>> GetAdjustments(
        [FromQuery] PaymentAdjustmentQueryRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _paymentService.GetAdjustmentsAsync(
            request, cancellationToken);

        return Ok(result);
    }

    // GET /api/v1/payments/adjustments/{id}
    [HttpGet("adjustments/{id:guid}")]
    [ProducesResponseType(typeof(PaymentAdjustmentResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PaymentAdjustmentResponse>> GetAdjustment(
        Guid id,
        CancellationToken cancellationToken)
    {
        var result = await _paymentService.GetAdjustmentAsync(
            id, cancellationToken);

        if (result is null)
            return NotFound();

        return Ok(result);
    }

    // POST /api/v1/payments/adjustments
    [HttpPost("adjustments")]
    [ProducesResponseType(typeof(PaymentAdjustmentResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PaymentAdjustmentResponse>> CreateAdjustment(
        [FromBody] CreatePaymentAdjustmentRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _paymentService.CreateAdjustmentAsync(
            request, cancellationToken);

        return CreatedAtAction(
            nameof(GetAdjustment),
            new { id = result.Id },
            result);
    }

    // PUT /api/v1/payments/adjustments/{id}
    [HttpPut("adjustments/{id:guid}")]
    [ProducesResponseType(typeof(PaymentAdjustmentResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PaymentAdjustmentResponse>> UpdateAdjustment(
        Guid id,
        [FromBody] UpdatePaymentAdjustmentRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _paymentService.UpdateAdjustmentAsync(
            id, request, cancellationToken);

        if (result is null)
            return NotFound();

        return Ok(result);
    }

    // GET /api/v1/payments/reconciliation
    [HttpGet("reconciliation")]
    [ProducesResponseType(typeof(PaymentReconciliationListResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<PaymentReconciliationListResponse>> GetReconciliations(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var result = await _paymentService.GetReconciliationsAsync(
            page, pageSize, cancellationToken);

        return Ok(result);
    }

    // POST /api/v1/payments/reconciliation/run
    [HttpPost("reconciliation/run")]
    [ProducesResponseType(typeof(PaymentReconciliationResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PaymentReconciliationResponse>> RunReconciliation(
        [FromBody] RunPaymentReconciliationRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _paymentService.RunReconciliationAsync(
            request, cancellationToken);

        return CreatedAtAction(
            nameof(GetReconciliations),
            new { page = 1, pageSize = 20 },
            result);
    }

    // GET /api/v1/payments/refunds
    [HttpGet("refunds")]
    [ProducesResponseType(typeof(IReadOnlyList<RefundResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<RefundResponse>>> GetRefunds(
        [FromQuery] RefundQueryRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _paymentService.GetRefundsAsync(
            request, cancellationToken);

        return Ok(result);
    }

    // GET /api/v1/payments/refunds/{id}
    [HttpGet("refunds/{id:guid}")]
    [ProducesResponseType(typeof(RefundResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<RefundResponse>> GetRefund(
        Guid id,
        CancellationToken cancellationToken)
    {
        var result = await _paymentService.GetRefundAsync(
            id, cancellationToken);

        if (result is null)
            return NotFound();

        return Ok(result);
    }

    // POST /api/v1/payments/refunds
    [HttpPost("refunds")]
    [ProducesResponseType(typeof(RefundResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<RefundResponse>> CreateRefund(
        [FromBody] CreateRefundRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _paymentService.CreateRefundAsync(
            request, cancellationToken);

        return CreatedAtAction(
            nameof(GetRefund),
            new { id = result.Id },
            result);
    }

    // POST /api/v1/payments/refunds/{id}/approve
    [HttpPost("refunds/{id:guid}/approve")]
    [ProducesResponseType(typeof(RefundResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<RefundResponse>> ApproveRefund(
        Guid id,
        [FromBody] ReviewRefundRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _paymentService.ApproveRefundAsync(
            id, request, cancellationToken);

        if (result is null)
            return NotFound();

        return Ok(result);
    }

    // POST /api/v1/payments/refunds/{id}/reject
    [HttpPost("refunds/{id:guid}/reject")]
    [ProducesResponseType(typeof(RefundResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<RefundResponse>> RejectRefund(
        Guid id,
        [FromBody] ReviewRefundRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _paymentService.RejectRefundAsync(
            id, request, cancellationToken);

        if (result is null)
            return NotFound();

        return Ok(result);
    }

}
