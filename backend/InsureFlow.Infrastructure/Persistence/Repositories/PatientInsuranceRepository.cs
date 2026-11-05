using InsureFlow.Application.Persistence;
using InsureFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace InsureFlow.Infrastructure.Persistence.Repositories;

public sealed class PatientInsuranceRepository(
    InsureFlowDbContext dbContext)
    : IPatientInsuranceRepository
{
    public async Task<IReadOnlyList<PatientInsurance>> GetPagedAsync(
        long patientId,
        string? search,
        string? status,
        string? insuranceType,
        string? coveragePriority,
        string? eligibilityStatus,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var query = dbContext.PatientInsurances
            .AsNoTracking()
            .Include(x => x.Patient)
            .Include(x => x.Payer)
            .Include(x => x.InsurancePlan)
            .Where(x => x.PatientId == patientId);

        if (!string.IsNullOrWhiteSpace(search))
        {
            var normalizedSearch = search.Trim();

            query = query.Where(x =>
                x.MemberId.Contains(normalizedSearch) ||
                (x.GroupNumber != null &&
                 x.GroupNumber.Contains(normalizedSearch)) ||
                (x.SubscriberId != null &&
                 x.SubscriberId.Contains(normalizedSearch)) ||
                (x.PayerIdDisplay != null &&
                 x.PayerIdDisplay.Contains(normalizedSearch)));
        }

        if (!string.IsNullOrWhiteSpace(status) &&
            Enum.TryParse<
                InsureFlow.Domain.Enums.InsuranceStatus>(
                status,
                true,
                out var parsedStatus))
        {
            query = query.Where(x =>
                x.InsuranceStatus == parsedStatus);
        }

        if (!string.IsNullOrWhiteSpace(insuranceType) &&
            Enum.TryParse<
                InsureFlow.Domain.Enums.InsuranceType>(
                insuranceType,
                true,
                out var parsedInsuranceType))
        {
            query = query.Where(x =>
                x.InsuranceType == parsedInsuranceType);
        }

        if (!string.IsNullOrWhiteSpace(coveragePriority) &&
            Enum.TryParse<
                InsureFlow.Domain.Enums.CoveragePriority>(
                coveragePriority,
                true,
                out var parsedPriority))
        {
            query = query.Where(x =>
                x.CoveragePriority == parsedPriority);
        }

        if (!string.IsNullOrWhiteSpace(eligibilityStatus) &&
            Enum.TryParse<
                InsureFlow.Domain.Enums.EligibilityStatus>(
                eligibilityStatus,
                true,
                out var parsedEligibility))
        {
            query = query.Where(x =>
                x.EligibilityStatus == parsedEligibility);
        }

        return await query
            .OrderBy(x => x.CoveragePriority)
            .ThenBy(x => x.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    public async Task<PatientInsurance?> GetByExternalIdAsync(
        Guid externalId,
        CancellationToken cancellationToken = default)
    {
        return await dbContext.PatientInsurances
            .Include(x => x.Patient)
            .Include(x => x.Payer)
            .Include(x => x.InsurancePlan)
            .FirstOrDefaultAsync(
                x => x.ExternalId == externalId,
                cancellationToken);
    }

    public async Task<PatientInsurance?> GetByIdAsync(
        long id,
        CancellationToken cancellationToken = default)
    {
        return await dbContext.PatientInsurances
            .Include(x => x.Patient)
            .Include(x => x.Payer)
            .Include(x => x.InsurancePlan)
            .FirstOrDefaultAsync(
                x => x.Id == id,
                cancellationToken);
    }

    public async Task<bool> ExistsByMemberIdAsync(
        long patientId,
        string memberId,
        Guid? excludeExternalId = null,
        CancellationToken cancellationToken = default)
    {
        var normalizedMemberId = memberId.Trim();

        var query = dbContext.PatientInsurances
            .Where(x =>
                x.PatientId == patientId &&
                x.MemberId == normalizedMemberId);

        if (excludeExternalId.HasValue)
        {
            query = query.Where(x =>
                x.ExternalId != excludeExternalId.Value);
        }

        return await query.AnyAsync(cancellationToken);
    }

    public async Task<bool> ExistsByCoveragePriorityAsync(
        long patientId,
        string coveragePriority,
        Guid? excludeExternalId = null,
        CancellationToken cancellationToken = default)
    {
        if (!Enum.TryParse<
                InsureFlow.Domain.Enums.CoveragePriority>(
                coveragePriority,
                true,
                out var parsedPriority))
        {
            return false;
        }

        var query = dbContext.PatientInsurances
            .Where(x =>
                x.PatientId == patientId &&
                x.CoveragePriority == parsedPriority &&
                x.IsActive);

        if (excludeExternalId.HasValue)
        {
            query = query.Where(x =>
                x.ExternalId != excludeExternalId.Value);
        }

        return await query.AnyAsync(cancellationToken);
    }

    public async Task AddAsync(
        PatientInsurance patientInsurance,
        CancellationToken cancellationToken = default)
    {
        await dbContext.PatientInsurances.AddAsync(
            patientInsurance,
            cancellationToken);
    }

    public Task UpdateAsync(
        PatientInsurance patientInsurance,
        CancellationToken cancellationToken = default)
    {
        dbContext.PatientInsurances.Update(patientInsurance);

        return Task.CompletedTask;
    }

    public Task DeleteAsync(
        PatientInsurance patientInsurance,
        CancellationToken cancellationToken = default)
    {
        dbContext.PatientInsurances.Remove(patientInsurance);

        return Task.CompletedTask;
    }

    public Task SaveChangesAsync(
        CancellationToken cancellationToken = default)
    {
        return dbContext.SaveChangesAsync(cancellationToken);
    }
}