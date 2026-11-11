
using InsureFlow.Domain.Entities;

namespace InsureFlow.Application.Persistence;

public interface IDenialRepository
{
    Task<IReadOnlyList<Denial>> GetPagedAsync(
        string? search,
        string? status,
        string? denialCategory,
        long? claimId,
        DateTime? denialDateFrom,
        DateTime? denialDateTo,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default);

    Task<Denial?> GetByExternalIdAsync(
        Guid externalId,
        CancellationToken cancellationToken = default);

    Task<bool> ExistsByDenialNumberAsync(
        string denialNumber,
        CancellationToken cancellationToken = default);

    Task AddAsync(
        Denial denial,
        CancellationToken cancellationToken = default);

    void Update(Denial denial);

    Task<int> SaveChangesAsync(
        CancellationToken cancellationToken = default);

    Task<Guid?> GetClaimExternalIdAsync(
        long claimId,
        CancellationToken cancellationToken = default);


    Task<IReadOnlyList<DenialActivity>> GetActivitiesAsync(
        long denialId,
        CancellationToken cancellationToken = default);

    Task AddActivityAsync(
        DenialActivity activity,
        CancellationToken cancellationToken = default);

    // Denial Attachments

    Task<IReadOnlyList<DenialAttachment>> GetAttachmentsAsync(
        long denialId,
        CancellationToken cancellationToken = default);

    Task<DenialAttachment?> GetAttachmentByExternalIdAsync(
        long denialId,
        Guid attachmentExternalId,
        CancellationToken cancellationToken = default);

    Task AddAttachmentAsync(
        DenialAttachment attachment,
        CancellationToken cancellationToken = default);

    void RemoveAttachment(DenialAttachment attachment);

    // Denial Payer Responses

    Task<IReadOnlyList<DenialPayerResponse>> GetPayerResponsesAsync(
        long denialId,
        CancellationToken cancellationToken = default);

    Task AddPayerResponseAsync(
        DenialPayerResponse response,
        CancellationToken cancellationToken = default);

}
