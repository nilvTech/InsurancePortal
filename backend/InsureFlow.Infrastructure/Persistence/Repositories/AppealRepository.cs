
using InsureFlow.Application.Persistence;
using InsureFlow.Domain.Entities;
using InsureFlow.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace InsureFlow.Infrastructure.Persistence.Repositories;

public sealed class AppealRepository : IAppealRepository
{
    private readonly InsureFlowDbContext _dbContext;

    public AppealRepository(InsureFlowDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    // Main appeals

    public async Task<IReadOnlyList<Appeal>> GetPagedAsync(
        string? search,
        string? status,
        Guid? denialExternalId,
        int page,
        int pageSize,
        CancellationToken cancellationToken)
    {
        IQueryable<Appeal> query = _dbContext.Appeals
            .AsNoTracking();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim();

            query = query.Where(x =>
                x.AppealNumber.Contains(term) ||
                x.AppealReason.Contains(term));
        }

        if (!string.IsNullOrWhiteSpace(status))
        {
            query = query.Where(x => x.Status == status.Trim());
        }

        if (denialExternalId.HasValue)
        {
            query = query.Where(x =>
                _dbContext.Denials.Any(d =>
                    d.Id == x.DenialId &&
                    d.ExternalId == denialExternalId.Value));
        }

        return await query
            .OrderByDescending(x => x.CreatedAtUtc)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    public Task<Appeal?> GetByExternalIdAsync(
        Guid externalId,
        CancellationToken cancellationToken)
    {
        return _dbContext.Appeals
            .FirstOrDefaultAsync(
                x => x.ExternalId == externalId,
                cancellationToken);
    }

    public Task<bool> ExistsAppealNumberAsync(
        string appealNumber,
        CancellationToken cancellationToken)
    {
        return _dbContext.Appeals
            .AnyAsync(
                x => x.AppealNumber == appealNumber,
                cancellationToken);
    }

    public async Task AddAsync(
        Appeal appeal,
        CancellationToken cancellationToken)
    {
        await _dbContext.Appeals.AddAsync(
            appeal,
            cancellationToken);
    }

    // Denial lookup

    public Task<Denial?> GetDenialByExternalIdAsync(
        Guid externalId,
        CancellationToken cancellationToken)
    {
        return _dbContext.Denials
            .FirstOrDefaultAsync(
                x => x.ExternalId == externalId,
                cancellationToken);
    }

    public Task<Denial?> GetDenialByIdAsync(
        long denialId,
        CancellationToken cancellationToken)
    {
        return _dbContext.Denials
            .FirstOrDefaultAsync(
                x => x.Id == denialId,
                cancellationToken);
    }

    public async Task<Guid?> GetDenialExternalIdAsync(
        long denialId,
        CancellationToken cancellationToken)
    {
        return await _dbContext.Denials
            .Where(x => x.Id == denialId)
            .Select(x => (Guid?)x.ExternalId)
            .FirstOrDefaultAsync(cancellationToken);
    }

    // Documentation

    public async Task<IReadOnlyList<AppealDocumentation>>
        GetDocumentationAsync(
            long appealId,
            CancellationToken cancellationToken)
    {
        return await _dbContext.AppealDocumentation
            .AsNoTracking()
            .Where(x => x.AppealId == appealId)
            .OrderByDescending(x => x.CreatedAtUtc)
            .ToListAsync(cancellationToken);
    }

    public async Task AddDocumentationAsync(
        AppealDocumentation documentation,
        CancellationToken cancellationToken)
    {
        await _dbContext.AppealDocumentation.AddAsync(
            documentation,
            cancellationToken);
    }

    // Attachments

    public async Task<IReadOnlyList<AppealAttachment>>
        GetAttachmentsAsync(
            long appealId,
            CancellationToken cancellationToken)
    {
        return await _dbContext.AppealAttachments
            .AsNoTracking()
            .Where(x => x.AppealId == appealId)
            .OrderByDescending(x => x.UploadedAtUtc)
            .ToListAsync(cancellationToken);
    }

    public Task<AppealAttachment?> GetAttachmentByExternalIdAsync(
        long appealId,
        Guid attachmentExternalId,
        CancellationToken cancellationToken)
    {
        return _dbContext.AppealAttachments
            .FirstOrDefaultAsync(
                x => x.AppealId == appealId &&
                     x.ExternalId == attachmentExternalId,
                cancellationToken);
    }

    public async Task AddAttachmentAsync(
        AppealAttachment attachment,
        CancellationToken cancellationToken)
    {
        await _dbContext.AppealAttachments.AddAsync(
            attachment,
            cancellationToken);
    }

    public void RemoveAttachment(AppealAttachment attachment)
    {
        _dbContext.AppealAttachments.Remove(attachment);
    }

    // External reviews

    public async Task<IReadOnlyList<AppealExternalReview>>
        GetExternalReviewsAsync(
            long? appealId,
            string? status,
            int page,
            int pageSize,
            CancellationToken cancellationToken)
    {
        IQueryable<AppealExternalReview> query =
            _dbContext.AppealExternalReviews.AsNoTracking();

        if (appealId.HasValue)
        {
            query = query.Where(x => x.AppealId == appealId.Value);
        }

        if (!string.IsNullOrWhiteSpace(status))
        {
            query = query.Where(x => x.Status == status.Trim());
        }

        return await query
            .OrderByDescending(x => x.RequestedAtUtc)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    public async Task AddExternalReviewAsync(
        AppealExternalReview externalReview,
        CancellationToken cancellationToken)
    {
        await _dbContext.AppealExternalReviews.AddAsync(
            externalReview,
            cancellationToken);
    }

    // History

    public async Task<IReadOnlyList<AppealHistory>> GetHistoryAsync(
        long appealId,
        CancellationToken cancellationToken)
    {
        return await _dbContext.AppealHistory
            .AsNoTracking()
            .Where(x => x.AppealId == appealId)
            .OrderByDescending(x => x.ActivityDateUtc)
            .ToListAsync(cancellationToken);
    }

    public async Task AddHistoryAsync(
        AppealHistory history,
        CancellationToken cancellationToken)
    {
        await _dbContext.AppealHistory.AddAsync(
            history,
            cancellationToken);
    }

    // Notifications

    public async Task<IReadOnlyList<AppealNotification>>
        GetNotificationsAsync(
            long appealId,
            CancellationToken cancellationToken)
    {
        return await _dbContext.AppealNotifications
            .Where(x => x.AppealId == appealId)
            .OrderByDescending(x => x.CreatedAtUtc)
            .ToListAsync(cancellationToken);
    }

    // Save changes

    public Task SaveChangesAsync(
        CancellationToken cancellationToken)
    {
        return _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<Guid?> GetAppealExternalIdAsync(
    long appealId,
    CancellationToken cancellationToken)
    {
        return await _dbContext.Appeals
            .Where(x => x.Id == appealId)
            .Select(x => (Guid?)x.ExternalId)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task AddNotificationAsync(
    AppealNotification notification,
    CancellationToken cancellationToken)
    {
        await _dbContext.AppealNotifications.AddAsync(
            notification,
            cancellationToken);
    }
}
