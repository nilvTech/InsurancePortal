
using InsureFlow.Domain.Entities;

namespace InsureFlow.Application.Persistence;

public interface IClaimRepository
{
    // Claims

    Task<IReadOnlyList<Claim>> GetPagedAsync(
        string? search,
        string? status,
        string? claimType,
        long? patientId,
        long? payerId,
        DateTime? serviceFrom,
        DateTime? serviceTo,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default);

    Task<Claim?> GetByExternalIdAsync(
        Guid externalId,
        CancellationToken cancellationToken = default);

    Task<(Guid PatientExternalId,
          Guid PatientInsuranceExternalId,
          Guid PayerExternalId,
          Guid ProviderExternalId)?> GetRelatedExternalIdsAsync(
        long patientId,
        long patientInsuranceId,
        long payerId,
        long providerId,
        CancellationToken cancellationToken = default);

    Task AddAsync(
        Claim claim,
        CancellationToken cancellationToken = default);

    Task UpdateAsync(
        Claim claim,
        CancellationToken cancellationToken = default);

    Task SaveChangesAsync(
        CancellationToken cancellationToken = default);

    // Claim Lines

    Task<IReadOnlyList<ClaimLine>> GetLinesByClaimIdAsync(
        long claimId,
        CancellationToken cancellationToken = default);

    Task<ClaimLine?> GetLineByExternalIdAsync(
        long claimId,
        Guid lineExternalId,
        CancellationToken cancellationToken = default);

    Task AddLinesAsync(
        IReadOnlyCollection<ClaimLine> lines,
        CancellationToken cancellationToken = default);

    Task ReplaceLinesAsync(
        long claimId,
        IReadOnlyCollection<ClaimLine> lines,
        CancellationToken cancellationToken = default);

    Task AddLineAsync(
        ClaimLine line,
        CancellationToken cancellationToken = default);

    Task UpdateLineAsync(
        ClaimLine line,
        CancellationToken cancellationToken = default);

    Task DeleteLineAsync(
        ClaimLine line,
        CancellationToken cancellationToken = default);

    // Claim History

    Task<IReadOnlyList<ClaimHistory>> GetHistoryAsync(
        long claimId,
        CancellationToken cancellationToken = default);

    Task AddHistoryAsync(
        ClaimHistory history,
        CancellationToken cancellationToken = default);

    // Claim Rejections

    Task<IReadOnlyList<ClaimRejection>> GetRejectionsByClaimIdAsync(
        long claimId,
        CancellationToken cancellationToken = default);

    Task<ClaimRejection?> GetRejectionByIdAsync(
        long claimId,
        long rejectionId,
        CancellationToken cancellationToken = default);

    Task AddRejectionAsync(
        ClaimRejection rejection,
        CancellationToken cancellationToken = default);

    Task UpdateRejectionAsync(
        ClaimRejection rejection,
        CancellationToken cancellationToken = default);

    // Claim Diagnoses

    Task<IReadOnlyList<ClaimDiagnosis>> GetDiagnosesByClaimIdAsync(
        long claimId,
        CancellationToken cancellationToken = default);

    Task AddDiagnosisAsync(
        ClaimDiagnosis diagnosis,
        CancellationToken cancellationToken = default);

    Task DeleteDiagnosisAsync(
        ClaimDiagnosis diagnosis,
        CancellationToken cancellationToken = default);

    // Claim Attachments

    Task<IReadOnlyList<ClaimAttachment>> GetAttachmentsByClaimIdAsync(
        long claimId,
        CancellationToken cancellationToken = default);

    Task<ClaimAttachment?> GetAttachmentByIdAsync(
        long claimId,
        long attachmentId,
        CancellationToken cancellationToken = default);

    Task AddAttachmentAsync(
        ClaimAttachment attachment,
        CancellationToken cancellationToken = default);

    Task DeleteAttachmentAsync(
        ClaimAttachment attachment,
        CancellationToken cancellationToken = default);
}
