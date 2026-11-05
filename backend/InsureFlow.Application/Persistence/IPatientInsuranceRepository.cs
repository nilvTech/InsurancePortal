using InsureFlow.Domain.Entities;

namespace InsureFlow.Application.Persistence;

public interface IPatientInsuranceRepository
{
    Task<IReadOnlyList<PatientInsurance>> GetPagedAsync(
        long patientId,
        string? search,
        string? status,
        string? insuranceType,
        string? coveragePriority,
        string? eligibilityStatus,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default);

    Task<PatientInsurance?> GetByExternalIdAsync(
        Guid externalId,
        CancellationToken cancellationToken = default);
        
    Task<PatientInsurance?> GetByIdAsync(
        long id,
        CancellationToken cancellationToken = default);

    Task<bool> ExistsByMemberIdAsync(
        long patientId,
        string memberId,
        Guid? excludeExternalId = null,
        CancellationToken cancellationToken = default);

    Task<bool> ExistsByCoveragePriorityAsync(
        long patientId,
        string coveragePriority,
        Guid? excludeExternalId = null,
        CancellationToken cancellationToken = default);

    Task AddAsync(
        PatientInsurance patientInsurance,
        CancellationToken cancellationToken = default);

    Task UpdateAsync(
        PatientInsurance patientInsurance,
        CancellationToken cancellationToken = default);

    Task DeleteAsync(
        PatientInsurance patientInsurance,
        CancellationToken cancellationToken = default);

    Task SaveChangesAsync(
        CancellationToken cancellationToken = default);
}