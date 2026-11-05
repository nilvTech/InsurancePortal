using InsureFlow.Application.Persistence;
using InsureFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace InsureFlow.Infrastructure.Persistence.Repositories;

public sealed class PatientRepository : IPatientRepository
{
    private readonly InsureFlowDbContext _dbContext;

    public PatientRepository(
        InsureFlowDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<Patient>> GetPagedAsync(
        string? search,
        string? status,
        string? gender,
        string? patientClass,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var query = _dbContext.Patients
            .AsNoTracking()
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var normalizedSearch = search.Trim();

            query = query.Where(x =>
                x.PatientId.Contains(normalizedSearch) ||
                x.Mrn.Contains(normalizedSearch) ||
                x.AccountNumber.Contains(normalizedSearch) ||
                x.FirstName.Contains(normalizedSearch) ||
                (x.MiddleName != null &&
                 x.MiddleName.Contains(normalizedSearch)) ||
                x.LastName.Contains(normalizedSearch) ||
                (x.PreferredName != null &&
                 x.PreferredName.Contains(normalizedSearch)));
        }

        if (!string.IsNullOrWhiteSpace(status))
        {
            var normalizedStatus = status.Trim();

            query = query.Where(x =>
                x.Status.ToString() == normalizedStatus);
        }

        if (!string.IsNullOrWhiteSpace(gender))
        {
            var normalizedGender = gender.Trim();

            query = query.Where(x =>
                x.Gender.ToString() == normalizedGender);
        }

        if (!string.IsNullOrWhiteSpace(patientClass))
        {
            var normalizedPatientClass =
                patientClass.Trim();

            query = query.Where(x =>
                x.PatientClass.ToString() ==
                normalizedPatientClass);
        }

        return await query
            .OrderBy(x => x.LastName)
            .ThenBy(x => x.FirstName)
            .ThenBy(x => x.PatientId)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    public async Task<Patient?> GetByExternalIdAsync(
        Guid externalId,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.Patients
            .FirstOrDefaultAsync(
                x => x.ExternalId == externalId,
                cancellationToken);
    }

    public async Task<Patient?> GetByIdAsync(
        long id,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.Patients
            .FirstOrDefaultAsync(
                x => x.Id == id,
                cancellationToken);
    }

    public async Task<bool> ExistsByPatientIdAsync(
        string patientId,
        Guid? excludeExternalId = null,
        CancellationToken cancellationToken = default)
    {
        var query = _dbContext.Patients
            .Where(x => x.PatientId == patientId);

        if (excludeExternalId.HasValue)
        {
            query = query.Where(
                x => x.ExternalId !=
                     excludeExternalId.Value);
        }

        return await query.AnyAsync(cancellationToken);
    }

    public async Task<bool> ExistsByMrnAsync(
        string mrn,
        Guid? excludeExternalId = null,
        CancellationToken cancellationToken = default)
    {
        var query = _dbContext.Patients
            .Where(x => x.Mrn == mrn);

        if (excludeExternalId.HasValue)
        {
            query = query.Where(
                x => x.ExternalId !=
                     excludeExternalId.Value);
        }

        return await query.AnyAsync(cancellationToken);
    }

    public async Task<bool> ExistsByAccountNumberAsync(
        string accountNumber,
        Guid? excludeExternalId = null,
        CancellationToken cancellationToken = default)
    {
        var query = _dbContext.Patients
            .Where(x =>
                x.AccountNumber == accountNumber);

        if (excludeExternalId.HasValue)
        {
            query = query.Where(
                x => x.ExternalId !=
                     excludeExternalId.Value);
        }

        return await query.AnyAsync(cancellationToken);
    }

    public async Task AddAsync(
        Patient patient,
        CancellationToken cancellationToken = default)
    {
        await _dbContext.Patients.AddAsync(
            patient,
            cancellationToken);
    }

    public Task UpdateAsync(
        Patient patient,
        CancellationToken cancellationToken = default)
    {
        _dbContext.Patients.Update(patient);

        return Task.CompletedTask;
    }

    public async Task SaveChangesAsync(
        CancellationToken cancellationToken = default)
    {
        await _dbContext.SaveChangesAsync(
            cancellationToken);
    }
}
