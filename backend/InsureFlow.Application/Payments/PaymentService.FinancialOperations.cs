
using InsureFlow.Application.Persistence;
using InsureFlow.Contracts.Payments;
using InsureFlow.Domain.Entities;

namespace InsureFlow.Application.Payments;

public sealed partial class PaymentService
{
    // 16. Get adjustments
    public async Task<IReadOnlyList<PaymentAdjustmentResponse>> GetAdjustmentsAsync(
        PaymentAdjustmentQueryRequest request,
        CancellationToken cancellationToken = default)
    {
        var records = await _repository.GetAdjustmentsAsync(cancellationToken);

        var filtered = records.AsEnumerable();

        if (!string.IsNullOrWhiteSpace(request.Status))
        {
            filtered = filtered.Where(x =>
                string.Equals(x.Status, request.Status,
                    StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrWhiteSpace(request.AdjustmentCode))
        {
            filtered = filtered.Where(x =>
                string.Equals(x.AdjustmentCode, request.AdjustmentCode,
                    StringComparison.OrdinalIgnoreCase));
        }

        return await MapAdjustmentsAsync(
            filtered.OrderByDescending(x => x.AdjustmentDateUtc).ToList(),
            cancellationToken);
    }

    // 17. Get adjustment
    public async Task<PaymentAdjustmentResponse?> GetAdjustmentAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var record = await _repository.GetAdjustmentAsync(id, cancellationToken);

        return record == null
            ? null
            : await MapAdjustmentAsync(record, cancellationToken);
    }

    // 18. Create adjustment
    public async Task<PaymentAdjustmentResponse> CreateAdjustmentAsync(
        CreatePaymentAdjustmentRequest request,
        CancellationToken cancellationToken = default)
    {
        var payment = await _repository.GetPaymentAsync(
            request.PaymentId, cancellationToken)
            ?? throw new InvalidOperationException("Payment was not found.");

        Claim? claim = null;
        PaymentAllocation? allocation = null;

        if (request.ClaimId.HasValue)
        {
            claim = await _repository.GetClaimAsync(
                request.ClaimId.Value, cancellationToken)
                ?? throw new InvalidOperationException("Claim was not found.");
        }

        if (request.PaymentAllocationId.HasValue)
        {
            allocation = await _repository.GetAllocationAsync(
                payment.Id,
                request.PaymentAllocationId.Value,
                cancellationToken)
                ?? throw new InvalidOperationException(
                    "Payment allocation was not found.");

            if (claim != null && allocation.ClaimId != claim.Id)
                throw new InvalidOperationException(
                    "The allocation does not belong to the selected claim.");

            claim ??= await _repository.GetClaimByIdAsync(
                allocation.ClaimId, cancellationToken);
        }

        if (request.AdjustmentAmount < 0)
            throw new InvalidOperationException(
                "Adjustment amount cannot be negative.");

        var adjustment = new PaymentAdjustment
        {
            TenantId = GetTenantId(),
            PaymentId = payment.Id,
            PaymentAllocationId = allocation?.Id,
            ClaimId = claim?.Id,
            AdjustmentCode = request.AdjustmentCode,
            AdjustmentGroupCode = request.AdjustmentGroupCode,
            Description = request.Description,
            AdjustmentAmount = request.AdjustmentAmount,
            AdjustmentDateUtc = request.AdjustmentDateUtc,
            Status = "RECORDED",
            Notes = request.Notes
        };

        await _repository.AddAdjustmentAsync(adjustment, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);

        return await MapAdjustmentAsync(adjustment, cancellationToken);
    }

    // 19. Update adjustment
    public async Task<PaymentAdjustmentResponse?> UpdateAdjustmentAsync(
        Guid id,
        UpdatePaymentAdjustmentRequest request,
        CancellationToken cancellationToken = default)
    {
        var adjustment = await _repository.GetAdjustmentAsync(
            id, cancellationToken);

        if (adjustment == null)
            return null;

        if (adjustment.Status != "RECORDED")
            throw new InvalidOperationException(
                "Only recorded adjustments can be updated.");

        if (request.AdjustmentAmount < 0)
            throw new InvalidOperationException(
                "Adjustment amount cannot be negative.");

        adjustment.AdjustmentCode = request.AdjustmentCode;
        adjustment.AdjustmentGroupCode = request.AdjustmentGroupCode;
        adjustment.Description = request.Description;
        adjustment.AdjustmentAmount = request.AdjustmentAmount;
        adjustment.AdjustmentDateUtc = request.AdjustmentDateUtc;
        adjustment.Notes = request.Notes;

        await _repository.SaveChangesAsync(cancellationToken);

        return await MapAdjustmentAsync(adjustment, cancellationToken);
    }

    // 20. Get reconciliation records
    public async Task<PaymentReconciliationListResponse> GetReconciliationsAsync(
        int page = 1,
        int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var records = await _repository.GetReconciliationsAsync(
            cancellationToken);

        page = Math.Max(1, page);
        pageSize = Math.Clamp(pageSize, 1, 100);

        var totalCount = records.Count;

        var items = records
            .OrderByDescending(x => x.StartedAtUtc)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(MapReconciliation)
            .ToList();

        return new PaymentReconciliationListResponse(
            items, page, pageSize, totalCount);
    }

    // 21. Run reconciliation
    public async Task<PaymentReconciliationResponse> RunReconciliationAsync(
        RunPaymentReconciliationRequest request,
        CancellationToken cancellationToken = default)
    {
        if (request.PeriodEndUtc <= request.PeriodStartUtc)
            throw new InvalidOperationException(
                "Reconciliation period end must be after its start.");

        var startedAt = DateTime.UtcNow;

        var payments = await _repository.GetPaymentsAsync(cancellationToken);

        var periodPayments = payments
            .Where(x =>
                x.PaymentDateUtc >= request.PeriodStartUtc &&
                x.PaymentDateUtc <= request.PeriodEndUtc &&
                !string.Equals(x.Status, "REVERSED",
                    StringComparison.OrdinalIgnoreCase))
            .ToList();

        var expectedAmount = periodPayments.Sum(x => x.PaymentAmount);
        var actualAmount = periodPayments.Sum(x => x.AllocatedAmount);
        var difference = expectedAmount - actualAmount;

        var reconciliation = new PaymentReconciliation
        {
            TenantId = GetTenantId(),
            ReconciliationNumber =
                $"REC-{startedAt:yyyyMMddHHmmss}-{Guid.NewGuid():N}"[..29],
            PeriodStartUtc = request.PeriodStartUtc,
            PeriodEndUtc = request.PeriodEndUtc,
            StartedAtUtc = startedAt,
            CompletedAtUtc = DateTime.UtcNow,
            ExpectedAmount = expectedAmount,
            ActualAmount = actualAmount,
            DifferenceAmount = difference,
            PaymentsProcessed = periodPayments.Count,
            PaymentsMatched = periodPayments.Count(
                x => x.UnallocatedAmount == 0),
            PaymentsUnmatched = periodPayments.Count(
                x => x.UnallocatedAmount != 0),
            Status = difference == 0 ? "COMPLETED" : "COMPLETED_WITH_VARIANCE",
            Summary = $"Processed {periodPayments.Count} payments. " +
                      $"Expected: {expectedAmount}; allocated: {actualAmount}."
        };

        await _repository.AddReconciliationAsync(
            reconciliation, cancellationToken);

        await _repository.SaveChangesAsync(cancellationToken);

        return MapReconciliation(reconciliation);
    }

    // 22. Get refunds
    public async Task<IReadOnlyList<RefundResponse>> GetRefundsAsync(
        RefundQueryRequest request,
        CancellationToken cancellationToken = default)
    {
        var records = await _repository.GetRefundsAsync(cancellationToken);

        var filtered = records.AsEnumerable();

        if (!string.IsNullOrWhiteSpace(request.Status))
        {
            filtered = filtered.Where(x =>
                string.Equals(x.Status, request.Status,
                    StringComparison.OrdinalIgnoreCase));
        }

        return await MapRefundsAsync(
            filtered.OrderByDescending(x => x.RequestedAtUtc).ToList(),
            cancellationToken);
    }

    // 23. Get refund
    public async Task<RefundResponse?> GetRefundAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var refund = await _repository.GetRefundAsync(id, cancellationToken);

        return refund == null
            ? null
            : await MapRefundAsync(refund, cancellationToken);
    }

    // 24. Create refund
    public async Task<RefundResponse> CreateRefundAsync(
        CreateRefundRequest request,
        CancellationToken cancellationToken = default)
    {
        var payment = await _repository.GetPaymentAsync(
            request.PaymentId, cancellationToken)
            ?? throw new InvalidOperationException("Payment was not found.");

        if (request.RefundAmount <= 0)
            throw new InvalidOperationException(
                "Refund amount must be greater than zero.");

        if (request.RefundAmount > payment.PaymentAmount)
            throw new InvalidOperationException(
                "Refund amount cannot exceed the original payment amount.");

        var refund = new Refund
        {
            TenantId = GetTenantId(),
            PaymentId = payment.Id,
            RefundNumber = $"REF-{DateTime.UtcNow:yyyyMMddHHmmss}-{Guid.NewGuid():N}"[..29],
            RefundAmount = request.RefundAmount,
            Reason = request.Reason,
            Status = "PENDING",
            RequestedAtUtc = DateTime.UtcNow,
            RequestedByUserId = null
        };

        await _repository.AddRefundAsync(refund, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);

        return await MapRefundAsync(refund, cancellationToken);
    }

    // 25. Approve refund
    public async Task<RefundResponse?> ApproveRefundAsync(
        Guid id,
        ReviewRefundRequest request,
        CancellationToken cancellationToken = default)
    {
        var refund = await _repository.GetRefundAsync(id, cancellationToken);

        if (refund == null)
            return null;

        refund.Approve(null, request.ReviewNotes);
        await _repository.SaveChangesAsync(cancellationToken);

        return await MapRefundAsync(refund, cancellationToken);
    }

    // 26. Reject refund
    public async Task<RefundResponse?> RejectRefundAsync(
        Guid id,
        ReviewRefundRequest request,
        CancellationToken cancellationToken = default)
    {
        var refund = await _repository.GetRefundAsync(id, cancellationToken);

        if (refund == null)
            return null;

        refund.Reject(null, request.ReviewNotes);
        await _repository.SaveChangesAsync(cancellationToken);

        return await MapRefundAsync(refund, cancellationToken);
    }

    // Adjustment mapping
   private async Task<PaymentAdjustmentResponse> MapAdjustmentAsync(
    PaymentAdjustment adjustment,
    CancellationToken cancellationToken)
{
    var payment = await _repository.GetPaymentByIdAsync(
        adjustment.PaymentId, cancellationToken);

    var allocations = await _repository.GetAllocationsAsync(
        adjustment.PaymentId, cancellationToken);

    var allocation = adjustment.PaymentAllocationId.HasValue
        ? allocations.FirstOrDefault(
            x => x.Id == adjustment.PaymentAllocationId.Value)
        : null;

    var claim = adjustment.ClaimId.HasValue
        ? await _repository.GetClaimByIdAsync(
            adjustment.ClaimId.Value, cancellationToken)
        : null;

    return new PaymentAdjustmentResponse(
        adjustment.ExternalId,
        payment?.ExternalId ?? Guid.Empty,
        allocation?.ExternalId,
        claim?.ExternalId,
        adjustment.AdjustmentCode,
        adjustment.AdjustmentGroupCode,
        adjustment.Description,
        adjustment.AdjustmentAmount,
        adjustment.AdjustmentDateUtc,
        adjustment.Status,
        adjustment.Notes);
}
    private async Task<List<PaymentAdjustmentResponse>> MapAdjustmentsAsync(
        List<PaymentAdjustment> records,
        CancellationToken cancellationToken)
    {
        var responses = new List<PaymentAdjustmentResponse>();

        foreach (var record in records)
            responses.Add(await MapAdjustmentAsync(record, cancellationToken));

        return responses;
    }

    // Reconciliation mapping
    private static PaymentReconciliationResponse MapReconciliation(
        PaymentReconciliation record)
    {
        return new PaymentReconciliationResponse(
            record.ExternalId,
            record.ReconciliationNumber,
            record.PeriodStartUtc,
            record.PeriodEndUtc,
            record.StartedAtUtc,
            record.CompletedAtUtc,
            record.ExpectedAmount,
            record.ActualAmount,
            record.DifferenceAmount,
            record.PaymentsProcessed,
            record.PaymentsMatched,
            record.PaymentsUnmatched,
            record.Status,
            record.Summary,
            record.ErrorMessage);
    }

    // Refund mapping
    private async Task<RefundResponse> MapRefundAsync(
        Refund refund,
        CancellationToken cancellationToken)
    {
        var payment = await _repository.GetPaymentByIdAsync(
            refund.PaymentId, cancellationToken);

        return new RefundResponse(
            refund.ExternalId,
            payment?.ExternalId ?? Guid.Empty,
            refund.RefundNumber,
            refund.RefundAmount,
            refund.Reason,
            refund.Status,
            refund.RequestedAtUtc,
            refund.RequestedByUserId,
            refund.ReviewedAtUtc,
            refund.ReviewedByUserId,
            refund.ReviewNotes,
            refund.ProcessedAtUtc);
    }

    private async Task<List<RefundResponse>> MapRefundsAsync(
        List<Refund> records,
        CancellationToken cancellationToken)
    {
        var responses = new List<RefundResponse>();

        foreach (var record in records)
            responses.Add(await MapRefundAsync(record, cancellationToken));

        return responses;
    }
}
