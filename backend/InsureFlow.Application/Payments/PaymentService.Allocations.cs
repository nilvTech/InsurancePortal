
using InsureFlow.Application.Persistence;
using InsureFlow.Contracts.Payments;
using InsureFlow.Domain.Entities;

namespace InsureFlow.Application.Payments;

public sealed partial class PaymentService
{
    // 7. Get payment allocations
    public async Task<IReadOnlyList<PaymentAllocationResponse>?> GetAllocationsAsync(
        Guid paymentId,
        CancellationToken cancellationToken = default)
    {
        var payment = await _repository.GetPaymentAsync(
            paymentId, cancellationToken);

        if (payment == null)
            return null;

        var allocations = await _repository.GetAllocationsAsync(
            payment.Id, cancellationToken);

        var responses = new List<PaymentAllocationResponse>();

        foreach (var allocation in allocations)
        {
            responses.Add(await MapAllocationAsync(
                allocation, cancellationToken));
        }

        return responses;
    }

    // 8. Create payment allocation
    public async Task<PaymentAllocationResponse?> CreateAllocationAsync(
        Guid paymentId,
        CreatePaymentAllocationRequest request,
        CancellationToken cancellationToken = default)
    {
        var payment = await _repository.GetPaymentAsync(
            paymentId, cancellationToken);

        if (payment == null)
            return null;

        if (string.Equals(payment.Status, "REVERSED",
                StringComparison.OrdinalIgnoreCase))
            throw new InvalidOperationException(
                "Allocations cannot be added to a reversed payment.");

        if (request.AllocatedAmount <= 0)
            throw new InvalidOperationException(
                "Allocation amount must be greater than zero.");

        if (request.AllocatedAmount > payment.UnallocatedAmount)
            throw new InvalidOperationException(
                "Allocation amount exceeds the unallocated payment amount.");

        var claim = await _repository.GetClaimAsync(
            request.ClaimId, cancellationToken)
            ?? throw new InvalidOperationException("Claim was not found.");

        ClaimLine? claimLine = null;

        if (request.ClaimLineId.HasValue)
        {
            claimLine = await _repository.GetClaimLineAsync(
                request.ClaimLineId.Value, cancellationToken)
                ?? throw new InvalidOperationException("Claim line was not found.");

            if (claimLine.ClaimId != claim.Id)
                throw new InvalidOperationException(
                    "The selected claim line does not belong to the selected claim.");
        }

        var allocation = new PaymentAllocation
        {
            TenantId = GetTenantId(),
            PaymentId = payment.Id,
            ClaimId = claim.Id,
            ClaimLineId = claimLine?.Id,
            AllocatedAmount = request.AllocatedAmount,
            AllocationDateUtc = DateTime.UtcNow,
            Status = "ACTIVE",
            Notes = request.Notes
        };

        payment.AllocatedAmount += request.AllocatedAmount;
        payment.UnallocatedAmount -= request.AllocatedAmount;

        await _repository.AddAllocationAsync(
            allocation, cancellationToken);

        await _repository.SaveChangesAsync(cancellationToken);

        return await MapAllocationAsync(allocation, cancellationToken);
    }

    // 9. Update payment allocation
    public async Task<PaymentAllocationResponse?> UpdateAllocationAsync(
        Guid paymentId,
        Guid allocationId,
        UpdatePaymentAllocationRequest request,
        CancellationToken cancellationToken = default)
    {
        var payment = await _repository.GetPaymentAsync(
            paymentId, cancellationToken);

        if (payment == null)
            return null;

        if (string.Equals(payment.Status, "REVERSED",
                StringComparison.OrdinalIgnoreCase))
            throw new InvalidOperationException(
                "Allocations cannot be updated on a reversed payment.");

        var allocation = await _repository.GetAllocationAsync(
            payment.Id, allocationId, cancellationToken);

        if (allocation == null)
            return null;

        if (request.AllocatedAmount <= 0)
            throw new InvalidOperationException(
                "Allocation amount must be greater than zero.");

        var availableAmount =
            payment.UnallocatedAmount + allocation.AllocatedAmount;

        if (request.AllocatedAmount > availableAmount)
            throw new InvalidOperationException(
                "Updated allocation exceeds the available payment amount.");

        var difference =
            request.AllocatedAmount - allocation.AllocatedAmount;

        allocation.AllocatedAmount = request.AllocatedAmount;
        allocation.Notes = request.Notes;

        payment.AllocatedAmount += difference;
        payment.UnallocatedAmount -= difference;

        await _repository.SaveChangesAsync(cancellationToken);

        return await MapAllocationAsync(allocation, cancellationToken);
    }

    // Allocation response mapping
    private async Task<PaymentAllocationResponse> MapAllocationAsync(
        PaymentAllocation allocation,
        CancellationToken cancellationToken)
    {
        var payment = await _repository.GetPaymentByIdAsync(
            allocation.PaymentId, cancellationToken);

        var claim = await _repository.GetClaimByIdAsync(
            allocation.ClaimId, cancellationToken);

        ClaimLine? claimLine = null;

        if (allocation.ClaimLineId.HasValue)
        {
            claimLine = await _repository.GetClaimLineByIdAsync(
                allocation.ClaimLineId.Value, cancellationToken);
        }

        return new PaymentAllocationResponse(
            allocation.ExternalId,
            payment?.ExternalId ?? Guid.Empty,
            claim?.ExternalId ?? Guid.Empty,
            claimLine?.ExternalId,
            allocation.AllocatedAmount,
            allocation.AllocationDateUtc,
            allocation.Status,
            allocation.Notes,
            allocation.CreatedAtUtc);
    }
}
