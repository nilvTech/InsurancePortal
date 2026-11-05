using InsureFlow.Application.Persistence;
using InsureFlow.Contracts.PatientInsurances;
using InsureFlow.Domain.Entities;
using InsureFlow.Domain.Enums;

namespace InsureFlow.Application.PatientInsurances;

public sealed class PatientInsuranceService(
    IPatientInsuranceRepository repository,
    IPatientRepository patientRepository,
    IPayerRepository payerRepository,
    IInsurancePlanRepository insurancePlanRepository)
    : IPatientInsuranceService
{
    public async Task<IReadOnlyList<PatientInsuranceDto>> GetPagedAsync(
        long patientId,
        PatientInsuranceListRequest request,
        CancellationToken cancellationToken = default)
    {
        var page = request.Page < 1
            ? 1
            : request.Page;

        var pageSize = request.PageSize switch
        {
            < 1 => 25,
            > 100 => 100,
            _ => request.PageSize
        };

        var records = await repository.GetPagedAsync(
            patientId,
            request.Search,
            request.Status,
            request.InsuranceType,
            request.CoveragePriority,
            request.EligibilityStatus,
            page,
            pageSize,
            cancellationToken);

        return records
            .Select(MapToDto)
            .ToList();
    }

    public async Task<PatientInsuranceDto?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var record = await repository.GetByExternalIdAsync(
            id,
            cancellationToken);

        return record is null
            ? null
            : MapToDto(record);
    }

    public async Task<PatientInsuranceDto> CreateAsync(
        CreatePatientInsuranceRequest request,
        CancellationToken cancellationToken = default)
    {
        if (!Enum.TryParse<InsuranceType>(
                request.InsuranceType,
                true,
                out var insuranceType))
        {
            throw new ArgumentException(
                "Invalid insurance type.",
                nameof(request.InsuranceType));
        }

        if (!Enum.TryParse<CoveragePriority>(
                request.CoveragePriority,
                true,
                out var coveragePriority))
        {
            throw new ArgumentException(
                "Invalid coverage priority.",
                nameof(request.CoveragePriority));
        }

        if (!Enum.TryParse<SubscriberRelationship>(
                request.SubscriberRelationship,
                true,
                out var subscriberRelationship))
        {
            throw new ArgumentException(
                "Invalid subscriber relationship.",
                nameof(request.SubscriberRelationship));
        }

        if (request.EffectiveDate.HasValue &&
            request.TerminationDate.HasValue &&
            request.TerminationDate.Value < request.EffectiveDate.Value)
        {
            throw new ArgumentException(
                "Termination date cannot be earlier than effective date.");
        }

        var patient =
            await patientRepository.GetByExternalIdAsync(
                request.PatientId,
                cancellationToken);

        if (patient is null)
        {
            throw new KeyNotFoundException(
                "Patient was not found.");
        }

        var payer =
            await payerRepository.GetByExternalIdAsync(
                request.PayerId,
                cancellationToken);

        if (payer is null)
        {
            throw new KeyNotFoundException(
                "Payer was not found.");
        }

        long? insurancePlanId = null;

        if (request.InsurancePlanId.HasValue)
        {
            var plan =
                await insurancePlanRepository.GetByExternalIdAsync(
                    request.InsurancePlanId.Value,
                    cancellationToken);

            if (plan is null)
            {
                throw new KeyNotFoundException(
                    "Insurance plan was not found.");
            }

            insurancePlanId = plan.Id;
        }

        if (await repository.ExistsByMemberIdAsync(
                patient.Id,
                request.MemberId,
                cancellationToken: cancellationToken))
        {
            throw new InvalidOperationException(
                "An insurance record with the same member ID " +
                "already exists for this patient.");
        }

        if (await repository.ExistsByCoveragePriorityAsync(
                patient.Id,
                request.CoveragePriority,
                cancellationToken: cancellationToken))
        {
            throw new InvalidOperationException(
                $"An active {request.CoveragePriority} insurance " +
                "already exists for this patient.");
        }

        var patientInsurance = new PatientInsurance(
            patient.TenantId,
            patient.Id,
            payer.Id,
            insurancePlanId,
            insuranceType,
            coveragePriority,
            request.MemberId);

        patientInsurance.UpdateDetails(
            payer.Id,
            insurancePlanId,
            insuranceType,
            coveragePriority,
            InsuranceStatus.PendingVerification,
            request.MemberId,
            request.GroupNumber,
            request.SubscriberId,
            request.SubscriberFirstName,
            request.SubscriberLastName,
            subscriberRelationship,
            request.EffectiveDate,
            request.TerminationDate,
            request.PayerIdDisplay);

        await repository.AddAsync(
            patientInsurance,
            cancellationToken);

        await repository.SaveChangesAsync(
            cancellationToken);

        var created =
            await repository.GetByExternalIdAsync(
                patientInsurance.ExternalId,
                cancellationToken);

        if (created is null)
        {
            throw new InvalidOperationException(
                "The patient insurance record could not be loaded after creation.");
        }

        return MapToDto(created);
    }

    public async Task<PatientInsuranceDto?> UpdateAsync(
        Guid id,
        UpdatePatientInsuranceRequest request,
        CancellationToken cancellationToken = default)
    {
        var patientInsurance =
            await repository.GetByExternalIdAsync(
                id,
                cancellationToken);

        if (patientInsurance is null)
        {
            return null;
        }

        if (!Enum.TryParse<InsuranceType>(
                request.InsuranceType,
                true,
                out var insuranceType))
        {
            throw new ArgumentException(
                "Invalid insurance type.",
                nameof(request.InsuranceType));
        }

        if (!Enum.TryParse<CoveragePriority>(
                request.CoveragePriority,
                true,
                out var coveragePriority))
        {
            throw new ArgumentException(
                "Invalid coverage priority.",
                nameof(request.CoveragePriority));
        }

        if (!Enum.TryParse<InsuranceStatus>(
                request.InsuranceStatus,
                true,
                out var insuranceStatus))
        {
            throw new ArgumentException(
                "Invalid insurance status.",
                nameof(request.InsuranceStatus));
        }

        if (!Enum.TryParse<SubscriberRelationship>(
                request.SubscriberRelationship,
                true,
                out var subscriberRelationship))
        {
            throw new ArgumentException(
                "Invalid subscriber relationship.",
                nameof(request.SubscriberRelationship));
        }

        if (request.EffectiveDate.HasValue &&
            request.TerminationDate.HasValue &&
            request.TerminationDate.Value < request.EffectiveDate.Value)
        {
            throw new ArgumentException(
                "Termination date cannot be earlier than effective date.");
        }

        var payer =
            await payerRepository.GetByExternalIdAsync(
                request.PayerId,
                cancellationToken);

        if (payer is null)
        {
            throw new KeyNotFoundException(
                "Payer was not found.");
        }

        long? insurancePlanId = null;

        if (request.InsurancePlanId.HasValue)
        {
            var plan =
                await insurancePlanRepository.GetByExternalIdAsync(
                    request.InsurancePlanId.Value,
                    cancellationToken);

            if (plan is null)
            {
                throw new KeyNotFoundException(
                    "Insurance plan was not found.");
            }

            insurancePlanId = plan.Id;
        }

        if (await repository.ExistsByMemberIdAsync(
                patientInsurance.PatientId,
                request.MemberId,
                id,
                cancellationToken))
        {
            throw new InvalidOperationException(
                "An insurance record with the same member ID " +
                "already exists for this patient.");
        }

        if (await repository.ExistsByCoveragePriorityAsync(
                patientInsurance.PatientId,
                request.CoveragePriority,
                id,
                cancellationToken))
        {
            throw new InvalidOperationException(
                $"An active {request.CoveragePriority} insurance " +
                "already exists for this patient.");
        }

        patientInsurance.UpdateDetails(
            payer.Id,
            insurancePlanId,
            insuranceType,
            coveragePriority,
            insuranceStatus,
            request.MemberId,
            request.GroupNumber,
            request.SubscriberId,
            request.SubscriberFirstName,
            request.SubscriberLastName,
            subscriberRelationship,
            request.EffectiveDate,
            request.TerminationDate,
            request.PayerIdDisplay);

        await repository.UpdateAsync(
            patientInsurance,
            cancellationToken);

        await repository.SaveChangesAsync(
            cancellationToken);

        return MapToDto(patientInsurance);
    }

    public async Task<bool> DeleteAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var record =
            await repository.GetByExternalIdAsync(
                id,
                cancellationToken);

        if (record is null)
        {
            return false;
        }

        await repository.DeleteAsync(
            record,
            cancellationToken);

        await repository.SaveChangesAsync(
            cancellationToken);

        return true;
    }

    public async Task<bool> ActivateAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var record =
            await repository.GetByExternalIdAsync(
                id,
                cancellationToken);

        if (record is null)
        {
            return false;
        }

        record.Activate();

        await repository.SaveChangesAsync(
            cancellationToken);

        return true;
    }

    public async Task<bool> DeactivateAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var record =
            await repository.GetByExternalIdAsync(
                id,
                cancellationToken);

        if (record is null)
        {
            return false;
        }

        record.Deactivate();

        await repository.SaveChangesAsync(
            cancellationToken);

        return true;
    }

    public async Task<bool> MarkEligibilityVerifiedAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var record =
            await repository.GetByExternalIdAsync(
                id,
                cancellationToken);

        if (record is null)
        {
            return false;
        }

        record.MarkEligibilityVerified(
            DateTime.UtcNow);

        await repository.SaveChangesAsync(
            cancellationToken);

        return true;
    }

    public async Task<bool> MarkEligibilityInactiveAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var record =
            await repository.GetByExternalIdAsync(
                id,
                cancellationToken);

        if (record is null)
        {
            return false;
        }

        record.MarkEligibilityInactive(
            DateTime.UtcNow);

        await repository.SaveChangesAsync(
            cancellationToken);

        return true;
    }

    public async Task<bool> MarkEligibilityErrorAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var record =
            await repository.GetByExternalIdAsync(
                id,
                cancellationToken);

        if (record is null)
        {
            return false;
        }

        record.MarkEligibilityError(
            DateTime.UtcNow);

        await repository.SaveChangesAsync(
            cancellationToken);

        return true;
    }

    private static PatientInsuranceDto MapToDto(
        PatientInsurance insurance)
    {
        return new PatientInsuranceDto(
            insurance.ExternalId,
            insurance.TenantId,
            insurance.Patient.ExternalId,
            BuildPatientName(insurance.Patient),
            insurance.Payer.ExternalId,
            insurance.Payer.Name,
            insurance.InsurancePlan?.ExternalId,
            insurance.InsurancePlan?.PlanName,
            insurance.InsuranceType.ToString(),
            insurance.CoveragePriority.ToString(),
            insurance.InsuranceStatus.ToString(),
            insurance.MemberId,
            insurance.GroupNumber,
            insurance.SubscriberId,
            insurance.SubscriberFirstName,
            insurance.SubscriberLastName,
            insurance.SubscriberRelationship.ToString(),
            insurance.EffectiveDate,
            insurance.TerminationDate,
            insurance.PayerIdDisplay,
            insurance.EligibilityStatus.ToString(),
            insurance.LastVerifiedAt,
            insurance.IsActive,
            insurance.CreatedAtUtc,
            insurance.UpdatedAtUtc);
    }

    private static string BuildPatientName(
        Patient patient)
    {
        return string.Join(
            " ",
            new[]
            {
                patient.FirstName,
                patient.MiddleName,
                patient.LastName
            }
            .Where(x => !string.IsNullOrWhiteSpace(x)));
    }
}