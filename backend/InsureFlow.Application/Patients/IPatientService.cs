using InsureFlow.Contracts.Patients;

namespace InsureFlow.Application.Patients;

public interface IPatientService
{
    Task<IReadOnlyList<PatientDto>> GetPagedAsync(
        PatientListRequest request,
        CancellationToken cancellationToken = default);

    Task<PatientDto?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<PatientDto> CreateAsync(
        CreatePatientRequest request,
        CancellationToken cancellationToken = default);

    Task<bool> UpdateAsync(
        Guid id,
        UpdatePatientRequest request,
        CancellationToken cancellationToken = default);

    Task<bool> DeactivateAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<bool> ActivateAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<bool> SetPortalAccessAsync(
        Guid id,
        string status,
        CancellationToken cancellationToken = default);

    Task<bool> SetCommunicationConsentAsync(
        Guid id,
        string status,
        CancellationToken cancellationToken = default);
}