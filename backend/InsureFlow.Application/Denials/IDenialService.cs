
using InsureFlow.Contracts.Denials;

namespace InsureFlow.Application.Denials;

public interface IDenialService
{
    Task<IReadOnlyList<DenialResponse>> GetPagedAsync(
        string? search,
        string? status,
        string? denialCategory,
        Guid? claimExternalId,
        DateTime? denialDateFrom,
        DateTime? denialDateTo,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default);

    Task<DenialResponse?> GetByExternalIdAsync(
        Guid externalId,
        CancellationToken cancellationToken = default);

    Task<DenialResponse> CreateAsync(
        CreateDenialRequest request,
        CancellationToken cancellationToken = default);

    Task<DenialResponse?> UpdateAsync(
        Guid externalId,
        UpdateDenialRequest request,
        CancellationToken cancellationToken = default);


    Task<DenialResponse?> AssignAsync(
        Guid externalId,
        long userId,
        CancellationToken cancellationToken = default);

    Task<DenialResponse?> UnassignAsync(
        Guid externalId,
        CancellationToken cancellationToken = default);


    Task<IReadOnlyList<DenialActivityResponse>> GetActivitiesAsync(
        Guid denialExternalId,
        CancellationToken cancellationToken = default);

    Task<DenialActivityResponse?> AddActivityAsync(
        Guid denialExternalId,
        DenialActivityRequest request,
        CancellationToken cancellationToken = default);
    // Denial Attachments

    Task<IReadOnlyList<DenialAttachmentResponse>> GetAttachmentsAsync(
        Guid denialExternalId,
        CancellationToken cancellationToken = default);

    Task<DenialAttachmentResponse?> AddAttachmentAsync(
        Guid denialExternalId,
        CreateDenialAttachmentRequest request,
        CancellationToken cancellationToken = default);

    Task<bool> DeleteAttachmentAsync(
        Guid denialExternalId,
        Guid attachmentExternalId,
        CancellationToken cancellationToken = default);
    // Denial Payer Responses

    Task<IReadOnlyList<DenialPayerResponseResponse>> GetPayerResponsesAsync(
        Guid denialExternalId,
        CancellationToken cancellationToken = default);

    Task<DenialPayerResponseResponse?> AddPayerResponseAsync(
        Guid denialExternalId,
        CreateDenialPayerResponseRequest request,
        CancellationToken cancellationToken = default);

    // Denial Resolutions

    Task<IReadOnlyList<DenialResolutionResponse>> GetResolutionsAsync(
        Guid denialExternalId,
        CancellationToken cancellationToken = default);

    Task<DenialResolutionResponse?> AddResolutionAsync(
        Guid denialExternalId,
        CreateDenialResolutionRequest request,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<DenialReasonResponse>> GetReasonsAsync(
bool? isActive,
CancellationToken cancellationToken = default);

    Task<DenialReasonResponse> CreateReasonAsync(
        CreateDenialReasonRequest request,
        CancellationToken cancellationToken = default);

    Task<DenialReasonResponse?> UpdateReasonAsync(
        Guid externalId,
        UpdateDenialReasonRequest request,
        CancellationToken cancellationToken = default);


    // Denial Appeal and History

    Task<DenialResponse?> AppealAsync(
        Guid denialExternalId,
        AppealDenialRequest request,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<DenialActivityResponse>> GetHistoryAsync(
        Guid denialExternalId,
        CancellationToken cancellationToken = default);

}
