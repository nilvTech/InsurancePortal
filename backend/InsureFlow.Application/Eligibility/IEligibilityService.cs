using InsureFlow.Contracts.Eligibility;

namespace InsureFlow.Application.Eligibility;

public interface IEligibilityService
{
    Task<EligibilityRequestDto> CreateAsync(
        CreateEligibilityRequest request,
        CancellationToken cancellationToken = default);

    Task<EligibilityRequestDto?> GetByIdAsync(
        long id,
        bool includeDetails = true,
        CancellationToken cancellationToken = default);

    Task<EligibilityRequestDto?> GetByExternalIdAsync(
        Guid externalId,
        bool includeDetails = true,
        CancellationToken cancellationToken = default);

    Task<EligibilityListResponse> GetPagedAsync(
        int pageNumber,
        int pageSize,
        string? status = null,
        long? patientId = null,
        long? patientInsuranceId = null,
        long? payerId = null,
        CancellationToken cancellationToken = default);

    Task<EligibilityRequestDto> MarkProcessingAsync(
        long id,
        CancellationToken cancellationToken = default);

    Task<EligibilityRequestDto> RecordResponseAsync(
        long id,
        RecordEligibilityResponseRequest request,
        CancellationToken cancellationToken = default);

    Task<EligibilityRequestDto> MarkFailedAsync(
        long id,
        string? errorCode,
        string? errorMessage,
        CancellationToken cancellationToken = default);

    Task<EligibilityRequestDto> CancelAsync(
        long id,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<EligibilityHistoryDto>>
        GetHistoryAsync(
            long requestId,
            CancellationToken cancellationToken = default);
    Task<EligibilityRequestDto> UpdateAsync(
long id,
UpdateEligibilityRequest request,
CancellationToken cancellationToken = default);

    Task<EligibilityRequestDto> SubmitAsync(
        long id,
        CancellationToken cancellationToken = default);

    Task<EligibilityRequestDto> RetryAsync(
        long id,
        CancellationToken cancellationToken = default);

    Task<EligibilityResponseDto?> GetResponseAsync(
        long requestId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<EligibilityBenefitDto>> GetBenefitsAsync(
        long requestId,
        CancellationToken cancellationToken = default);

    Task<EligibilityBenefitDto> AddBenefitAsync(
        long requestId,
        CreateEligibilityBenefitRequest request,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<EligibilityHistoryDto>> GetAllHistoryAsync(
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default);

    Task<EligibilityListResponse> GetFailuresAsync(
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default);

    Task<EligibilityListResponse> GetByPatientAsync(
        long patientId,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default);

    Task<EligibilityListResponse> GetByInsuranceAsync(
        long patientInsuranceId,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default);
}