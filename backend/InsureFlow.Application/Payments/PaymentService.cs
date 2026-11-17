
using InsureFlow.Application.Persistence;
using InsureFlow.Application.Tenants;
using InsureFlow.Contracts.Payments;
using InsureFlow.Domain.Entities;

namespace InsureFlow.Application.Payments;

public sealed partial class PaymentService : IPaymentService
{
    private readonly IPaymentRepository _repository;
    private readonly ITenantContext _tenantContext;

    public PaymentService(
        IPaymentRepository repository,
        ITenantContext tenantContext)
    {
        _repository = repository;
        _tenantContext = tenantContext;
    }

    private long GetTenantId()
    {
        if (!_tenantContext.HasTenant || !_tenantContext.TenantId.HasValue)
            throw new InvalidOperationException("A valid tenant context is required.");

        return _tenantContext.TenantId.Value;
    }

    // 1. Get payments
    public async Task<PaymentListResponse> GetPaymentsAsync(
        PaymentQueryRequest request,
        CancellationToken cancellationToken = default)
    {
        var payments = await _repository.GetPaymentsAsync(cancellationToken);

        var filtered = payments.AsEnumerable();

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var search = request.Search.Trim();

            filtered = filtered.Where(x =>
                x.PaymentNumber.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                (x.CheckNumber != null &&
                 x.CheckNumber.Contains(search, StringComparison.OrdinalIgnoreCase)) ||
                (x.TraceNumber != null &&
                 x.TraceNumber.Contains(search, StringComparison.OrdinalIgnoreCase)));
        }

        if (!string.IsNullOrWhiteSpace(request.Status))
            filtered = filtered.Where(x =>
                string.Equals(x.Status, request.Status, StringComparison.OrdinalIgnoreCase));

        if (request.FromDateUtc.HasValue)
            filtered = filtered.Where(x => x.PaymentDateUtc >= request.FromDateUtc.Value);

        if (request.ToDateUtc.HasValue)
            filtered = filtered.Where(x => x.PaymentDateUtc <= request.ToDateUtc.Value);

        if (request.PayerId.HasValue)
        {
            var payer = await _repository.GetPayerAsync(
                request.PayerId.Value, cancellationToken);

            if (payer == null)
                return new PaymentListResponse([], request.Page, request.PageSize, 0);

            filtered = filtered.Where(x => x.PayerId == payer.Id);
        }

        var totalCount = filtered.Count();
        var page = Math.Max(1, request.Page);
        var pageSize = Math.Clamp(request.PageSize, 1, 100);

        var items = filtered
            .OrderByDescending(x => x.PaymentDateUtc)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        var responses = new List<PaymentResponse>();

        foreach (var payment in items)
            responses.Add(await MapPaymentAsync(payment, cancellationToken));

        return new PaymentListResponse(
            responses, page, pageSize, totalCount);
    }

    // 2. Get payment by external ID
    public async Task<PaymentResponse?> GetPaymentAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var payment = await _repository.GetPaymentAsync(id, cancellationToken);

        return payment == null
            ? null
            : await MapPaymentAsync(payment, cancellationToken);
    }

    // 3. Create payment
    public async Task<PaymentResponse> CreatePaymentAsync(
        CreatePaymentRequest request,
        CancellationToken cancellationToken = default)
    {
        var tenantId = GetTenantId();

        var payer = await _repository.GetPayerAsync(
            request.PayerId, cancellationToken)
            ?? throw new InvalidOperationException("Payer was not found.");

        Patient? patient = null;
        PatientInsurance? insurance = null;

        if (request.PatientId.HasValue)
        {
            patient = await _repository.GetPatientAsync(
                request.PatientId.Value, cancellationToken)
                ?? throw new InvalidOperationException("Patient was not found.");
        }

        if (request.PatientInsuranceId.HasValue)
        {
            insurance = await _repository.GetPatientInsuranceAsync(
                request.PatientInsuranceId.Value, cancellationToken)
                ?? throw new InvalidOperationException("Patient insurance was not found.");

            if (patient != null && insurance.PatientId != patient.Id)
                throw new InvalidOperationException(
                    "The insurance record does not belong to the selected patient.");
        }

        if (request.PaymentAmount <= 0)
            throw new InvalidOperationException("Payment amount must be greater than zero.");

        var payment = new Payment
        {
            TenantId = tenantId,
            PaymentNumber = request.PaymentNumber,
            PayerId = payer.Id,
            PatientId = patient?.Id,
            PatientInsuranceId = insurance?.Id,
            CheckNumber = request.CheckNumber,
            TraceNumber = request.TraceNumber,
            PaymentMethod = request.PaymentMethod,
            PaymentAmount = request.PaymentAmount,
            AllocatedAmount = 0,
            UnallocatedAmount = request.PaymentAmount,
            Currency = request.Currency,
            PaymentDateUtc = request.PaymentDateUtc,
            ReceivedDateUtc = request.ReceivedDateUtc,
            Status = "DRAFT",
            Notes = request.Notes
        };

        await _repository.AddPaymentAsync(payment, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);

        return await MapPaymentAsync(payment, cancellationToken);
    }

    // 4. Update payment
    public async Task<PaymentResponse?> UpdatePaymentAsync(
        Guid id,
        UpdatePaymentRequest request,
        CancellationToken cancellationToken = default)
    {
        var payment = await _repository.GetPaymentAsync(id, cancellationToken);

        if (payment == null)
            return null;

        if (!string.Equals(payment.Status, "DRAFT", StringComparison.OrdinalIgnoreCase))
            throw new InvalidOperationException("Only draft payments can be updated.");

        if (request.PaymentAmount <= 0)
            throw new InvalidOperationException("Payment amount must be greater than zero.");

        payment.CheckNumber = request.CheckNumber;
        payment.TraceNumber = request.TraceNumber;
        payment.PaymentMethod = request.PaymentMethod;
        payment.PaymentAmount = request.PaymentAmount;
        payment.Currency = request.Currency;
        payment.PaymentDateUtc = request.PaymentDateUtc;
        payment.ReceivedDateUtc = request.ReceivedDateUtc;
        payment.Notes = request.Notes;
        payment.UnallocatedAmount = request.PaymentAmount - payment.AllocatedAmount;

        if (payment.UnallocatedAmount < 0)
            throw new InvalidOperationException(
                "Payment amount cannot be less than its allocated amount.");

        await _repository.SaveChangesAsync(cancellationToken);

        return await MapPaymentAsync(payment, cancellationToken);
    }

    // 5. Post payment
    public async Task<PaymentResponse?> PostPaymentAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var payment = await _repository.GetPaymentAsync(id, cancellationToken);

        if (payment == null)
            return null;

        payment.Post();
        await _repository.SaveChangesAsync(cancellationToken);

        return await MapPaymentAsync(payment, cancellationToken);
    }

    // 6. Reverse payment
    public async Task<PaymentResponse?> ReversePaymentAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var payment = await _repository.GetPaymentAsync(id, cancellationToken);

        if (payment == null)
            return null;

        payment.Reverse();
        await _repository.SaveChangesAsync(cancellationToken);

        return await MapPaymentAsync(payment, cancellationToken);
    }

    private async Task<PaymentResponse> MapPaymentAsync(
        Payment payment,
        CancellationToken cancellationToken)
    {
        var payer = await _repository.GetPayerByIdAsync(
            payment.PayerId, cancellationToken);

        var patient = payment.PatientId.HasValue
            ? await _repository.GetPatientByIdAsync(
                payment.PatientId.Value, cancellationToken)
            : null;

        var insurance = payment.PatientInsuranceId.HasValue
            ? await _repository.GetPatientInsuranceByIdAsync(
                payment.PatientInsuranceId.Value, cancellationToken)
            : null;

        return new PaymentResponse(
            payment.ExternalId,
            payment.PaymentNumber,
            payer?.ExternalId ?? Guid.Empty,
            patient?.ExternalId,
            insurance?.ExternalId,
            payment.CheckNumber,
            payment.TraceNumber,
            payment.PaymentMethod,
            payment.PaymentAmount,
            payment.AllocatedAmount,
            payment.UnallocatedAmount,
            payment.Currency,
            payment.PaymentDateUtc,
            payment.ReceivedDateUtc,
            payment.PostedAtUtc,
            payment.ReversedAtUtc,
            payment.Status,
            payment.Notes,
            payment.CreatedAtUtc);
    }
}
