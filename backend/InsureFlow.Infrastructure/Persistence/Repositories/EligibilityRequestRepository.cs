using InsureFlow.Application.Persistence;
using InsureFlow.Domain.Entities;
using InsureFlow.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace InsureFlow.Infrastructure.Persistence.Repositories;

public sealed class EligibilityRequestRepository(
    InsureFlowDbContext dbContext)
    : IEligibilityRequestRepository
{
    public async Task<EligibilityRequest?> GetByIdAsync(
        long id,
        bool includeDetails = false,
        CancellationToken cancellationToken = default)
    {
        IQueryable<EligibilityRequest> query =
            dbContext.EligibilityRequests;

        if (includeDetails)
        {
            query = query
                .Include(x => x.Response)
                    .ThenInclude(x => x!.Benefits)
                .Include(x => x.History);
        }

        return await query
            .FirstOrDefaultAsync(
                x => x.Id == id,
                cancellationToken);
    }

    public async Task<EligibilityRequest?> GetByRequestNumberAsync(
        string requestNumber,
        CancellationToken cancellationToken = default)
    {
        var normalizedRequestNumber =
            requestNumber.Trim();

        return await dbContext.EligibilityRequests
            .AsNoTracking()
            .FirstOrDefaultAsync(
                x => x.RequestNumber == normalizedRequestNumber,
                cancellationToken);
    }

    public async Task<EligibilityRequest?> GetByExternalIdAsync(
        Guid externalId,
        CancellationToken cancellationToken = default)
    {
        return await dbContext.EligibilityRequests
            .AsNoTracking()
            .FirstOrDefaultAsync(
                x => x.ExternalId == externalId,
                cancellationToken);
    }

    public async Task<IReadOnlyList<EligibilityRequest>> GetPagedAsync(
        int pageNumber,
        int pageSize,
        EligibilityRequestStatus? status = null,
        long? patientId = null,
        long? patientInsuranceId = null,
        long? payerId = null,
        CancellationToken cancellationToken = default)
    {
        if (pageNumber <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(pageNumber),
                "Page number must be greater than zero.");
        }

        if (pageSize <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(pageSize),
                "Page size must be greater than zero.");
        }

        IQueryable<EligibilityRequest> query =
            dbContext.EligibilityRequests
                .AsNoTracking();

        if (status.HasValue)
        {
            query = query.Where(
                x => x.Status == status.Value);
        }

        if (patientId.HasValue)
        {
            query = query.Where(
                x => x.PatientId == patientId.Value);
        }

        if (patientInsuranceId.HasValue)
        {
            query = query.Where(
                x => x.PatientInsuranceId ==
                     patientInsuranceId.Value);
        }

        if (payerId.HasValue)
        {
            query = query.Where(
                x => x.PayerId == payerId.Value);
        }

        return await query
            .OrderByDescending(x => x.RequestedAt)
            .ThenByDescending(x => x.Id)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    public async Task<int> CountAsync(
        EligibilityRequestStatus? status = null,
        long? patientId = null,
        long? patientInsuranceId = null,
        long? payerId = null,
        CancellationToken cancellationToken = default)
    {
        IQueryable<EligibilityRequest> query =
            dbContext.EligibilityRequests;

        if (status.HasValue)
        {
            query = query.Where(
                x => x.Status == status.Value);
        }

        if (patientId.HasValue)
        {
            query = query.Where(
                x => x.PatientId == patientId.Value);
        }

        if (patientInsuranceId.HasValue)
        {
            query = query.Where(
                x => x.PatientInsuranceId ==
                     patientInsuranceId.Value);
        }

        if (payerId.HasValue)
        {
            query = query.Where(
                x => x.PayerId == payerId.Value);
        }

        return await query.CountAsync(
            cancellationToken);
    }

    public async Task<bool> ExistsByRequestNumberAsync(
        string requestNumber,
        CancellationToken cancellationToken = default)
    {
        var normalizedRequestNumber =
            requestNumber.Trim();

        return await dbContext.EligibilityRequests
            .AnyAsync(
                x => x.RequestNumber ==
                     normalizedRequestNumber,
                cancellationToken);
    }

    public async Task AddAsync(
        EligibilityRequest entity,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entity);

        await dbContext.EligibilityRequests
            .AddAsync(
                entity,
                cancellationToken);
    }

    public void Update(
        EligibilityRequest entity)
    {
        ArgumentNullException.ThrowIfNull(entity);

        dbContext.EligibilityRequests.Update(entity);
    }

    public void Remove(
        EligibilityRequest entity)
    {
        ArgumentNullException.ThrowIfNull(entity);

        dbContext.EligibilityRequests.Remove(entity);
    }

    public Task<int> SaveChangesAsync(
        CancellationToken cancellationToken = default)
    {
        return dbContext.SaveChangesAsync(
            cancellationToken);
    }
}