using InsureFlow.Domain.Entities;
using InsureFlow.Domain.Enums;

namespace InsureFlow.Application.Persistence;

public interface IEligibilityRequestRepository
{
    Task<EligibilityRequest?> GetByIdAsync(
        long id,
        bool includeDetails = false,
        CancellationToken cancellationToken = default);

    Task<EligibilityRequest?> GetByRequestNumberAsync(
        string requestNumber,
        CancellationToken cancellationToken = default);

    Task<EligibilityRequest?> GetByExternalIdAsync(
        Guid externalId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<EligibilityRequest>> GetPagedAsync(
        int pageNumber,
        int pageSize,
        EligibilityRequestStatus? status = null,
        long? patientId = null,
        long? patientInsuranceId = null,
        long? payerId = null,
        CancellationToken cancellationToken = default);

    Task<int> CountAsync(
        EligibilityRequestStatus? status = null,
        long? patientId = null,
        long? patientInsuranceId = null,
        long? payerId = null,
        CancellationToken cancellationToken = default);

    Task<bool> ExistsByRequestNumberAsync(
        string requestNumber,
        CancellationToken cancellationToken = default);

    Task AddAsync(
        EligibilityRequest entity,
        CancellationToken cancellationToken = default);

    void Update(
        EligibilityRequest entity);

    void Remove(
        EligibilityRequest entity);

    Task<int> SaveChangesAsync(
        CancellationToken cancellationToken = default);
}