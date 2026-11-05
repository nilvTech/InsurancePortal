using InsureFlow.Contracts.Payers;

namespace InsureFlow.Application.Payers;

public interface IPayerService
{
    Task<IReadOnlyList<PayerDto>> GetPagedAsync(
        PayerListRequest request,
        CancellationToken cancellationToken = default);

    Task<PayerDto?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<PayerDto> CreateAsync(
        CreatePayerRequest request,
        CancellationToken cancellationToken = default);

    Task<bool> UpdateAsync(
        Guid id,
        UpdatePayerRequest request,
        CancellationToken cancellationToken = default);

    Task<bool> DeactivateAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<bool> ActivateAsync(
        Guid id,
        CancellationToken cancellationToken = default);
}