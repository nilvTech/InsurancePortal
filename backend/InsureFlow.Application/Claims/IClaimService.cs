
using InsureFlow.Contracts.Claims;

namespace InsureFlow.Application.Claims;

public interface IClaimService
{
    // Claims

    Task<IReadOnlyList<ClaimDto>> GetPagedAsync(
        ClaimListRequest request,
        CancellationToken cancellationToken = default);

    Task<ClaimDto?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<ClaimDto> CreateAsync(
        CreateClaimRequest request,
        CancellationToken cancellationToken = default);

    Task<bool> UpdateAsync(
        Guid id,
        UpdateClaimRequest request,
        CancellationToken cancellationToken = default);

    // Claim validation

    Task<ClaimValidationDto> ValidateAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<ClaimValidationDto?> GetValidationAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    // Claim submission

    Task<ClaimDto> SubmitAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<ClaimSubmissionResponseDto?> GetSubmissionResponseAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<ClaimDto> ResubmitAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<ClaimRejectionDto>?> GetRejectionsAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    // Claim lines

    Task<IReadOnlyList<ClaimLineDto>?> GetLinesAsync(
        Guid claimId,
        CancellationToken cancellationToken = default);

    Task<ClaimLineDto?> AddLineAsync(
        Guid claimId,
        ClaimLineInput request,
        CancellationToken cancellationToken = default);

    Task<ClaimLineDto?> UpdateLineAsync(
        Guid claimId,
        Guid lineId,
        ClaimLineInput request,
        CancellationToken cancellationToken = default);

    Task<bool> DeleteLineAsync(
        Guid claimId,
        Guid lineId,
        CancellationToken cancellationToken = default);

    // Claim history

    Task<IReadOnlyList<ClaimHistoryDto>?> GetHistoryAsync(
        Guid claimId,
        CancellationToken cancellationToken = default);

    // Claim diagnoses

    Task<IReadOnlyList<ClaimDiagnosisDto>?> GetDiagnosesAsync(
        Guid claimId,
        CancellationToken cancellationToken = default);

    Task<ClaimDiagnosisDto?> AddDiagnosisAsync(
        Guid claimId,
        ClaimDiagnosisInput request,
        CancellationToken cancellationToken = default);

    Task<bool> DeleteDiagnosisAsync(
        Guid claimId,
        long diagnosisId,
        CancellationToken cancellationToken = default);

    // Claim attachments

    Task<IReadOnlyList<ClaimAttachmentDto>?> GetAttachmentsAsync(
        Guid claimId,
        CancellationToken cancellationToken = default);

    Task<ClaimAttachmentDto?> AddAttachmentAsync(
        Guid claimId,
        ClaimAttachmentInput request,
        CancellationToken cancellationToken = default);

    Task<bool> DeleteAttachmentAsync(
        Guid claimId,
        long attachmentId,
        CancellationToken cancellationToken = default);

    // Claim status

    Task<IReadOnlyList<ClaimStatusDto>> GetPagedStatusesAsync(
        ClaimListRequest request,
        CancellationToken cancellationToken = default);

    Task<ClaimStatusDto?> GetStatusAsync(
        Guid claimId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<ClaimStatusHistoryDto>?> GetStatusHistoryAsync(
        Guid claimId,
        CancellationToken cancellationToken = default);
}
