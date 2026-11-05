using InsureFlow.Contracts.PatientInsurances;

namespace InsureFlow.Application.PatientInsurances;

public interface IPatientInsuranceService
{
    Task<IReadOnlyList<PatientInsuranceDto>> GetPagedAsync(
        long patientId,
        PatientInsuranceListRequest request,
        CancellationToken cancellationToken = default);

    Task<PatientInsuranceDto?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<PatientInsuranceDto> CreateAsync(
        CreatePatientInsuranceRequest request,
        CancellationToken cancellationToken = default);

    Task<PatientInsuranceDto?> UpdateAsync(
        Guid id,
        UpdatePatientInsuranceRequest request,
        CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<bool> ActivateAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<bool> DeactivateAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<bool> MarkEligibilityVerifiedAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<bool> MarkEligibilityInactiveAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<bool> MarkEligibilityErrorAsync(
        Guid id,
        CancellationToken cancellationToken = default);
}