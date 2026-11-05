using InsureFlow.Application.Abstractions;
using InsureFlow.Application.Persistence;
using InsureFlow.Application.Tenants;
using InsureFlow.Contracts.Patients;
using InsureFlow.Domain.Entities;
using InsureFlow.Domain.Enums;

namespace InsureFlow.Application.Patients;

public sealed class PatientService : IPatientService
{
    private readonly IPatientRepository _patientRepository;
    private readonly ITenantContext _tenantContext;
    private readonly IAuditService _auditService;

    public PatientService(
        IPatientRepository patientRepository,
        ITenantContext tenantContext,
        IAuditService auditService)
    {
        _patientRepository = patientRepository;
        _tenantContext = tenantContext;
        _auditService = auditService;
    }

    public async Task<IReadOnlyList<PatientDto>> GetPagedAsync(
        PatientListRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var page = request.Page < 1
            ? 1
            : request.Page;

        var pageSize = request.PageSize switch
        {
            < 1 => 25,
            > 100 => 100,
            _ => request.PageSize
        };

        var patients =
            await _patientRepository.GetPagedAsync(
                request.Search,
                request.Status,
                request.Gender,
                request.PatientClass,
                page,
                pageSize,
                cancellationToken);

        return patients
            .Select(MapToDto)
            .ToList();
    }

    public async Task<PatientDto?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException(
                "Patient ID is required.",
                nameof(id));
        }

        var patient =
            await _patientRepository.GetByExternalIdAsync(
                id,
                cancellationToken);

        return patient is null
            ? null
            : MapToDto(patient);
    }

    public async Task<PatientDto> CreateAsync(
        CreatePatientRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var tenantId = RequireTenant();

        var patientId = NormalizeRequired(
            request.PatientId,
            nameof(request.PatientId),
            "Patient ID is required.");

        var mrn = NormalizeRequired(
            request.Mrn,
            nameof(request.Mrn),
            "MRN is required.");

        var accountNumber = NormalizeRequired(
            request.AccountNumber,
            nameof(request.AccountNumber),
            "Account number is required.");

        var firstName = NormalizeRequired(
            request.FirstName,
            nameof(request.FirstName),
            "Patient first name is required.");

        var lastName = NormalizeRequired(
            request.LastName,
            nameof(request.LastName),
            "Patient last name is required.");

        ValidateDateOfBirth(request.DateOfBirth);

        if (await _patientRepository.ExistsByPatientIdAsync(
                patientId,
                cancellationToken: cancellationToken))
        {
            throw new InvalidOperationException(
                $"A patient with ID '{patientId}' already exists.");
        }

        if (await _patientRepository.ExistsByMrnAsync(
                mrn,
                cancellationToken: cancellationToken))
        {
            throw new InvalidOperationException(
                $"A patient with MRN '{mrn}' already exists.");
        }

        if (await _patientRepository.ExistsByAccountNumberAsync(
                accountNumber,
                cancellationToken: cancellationToken))
        {
            throw new InvalidOperationException(
                $"A patient with account number '{accountNumber}' already exists.");
        }

        var gender = ParseEnum<PatientGender>(
            request.Gender,
            nameof(request.Gender));

        var patientClass = ParseEnum<PatientClass>(
            request.PatientClass,
            nameof(request.PatientClass));

        var referralSource =
            ParseNullableEnum<ReferralSource>(
                request.ReferralSource,
                nameof(request.ReferralSource));

        var patient = new Patient(
            tenantId,
            patientId,
            mrn,
            accountNumber,
            firstName,
            lastName,
            request.DateOfBirth);

        patient.UpdateDemographics(
            firstName,
            request.MiddleName,
            lastName,
            request.PreferredName,
            request.DateOfBirth,
            gender);

        patient.UpdatePatientInformation(
            request.RegistrationDate,
            request.LastVisitDate,
            patientClass,
            request.PreferredPharmacy,
            referralSource,
            request.Notes);

        await _patientRepository.AddAsync(
            patient,
            cancellationToken);

        await _patientRepository.SaveChangesAsync(
            cancellationToken);

        await _auditService.WriteAsync(
            new AuditEntry(
                TenantId: tenantId,
                UserId: null,
                Action: "PatientCreated",
                EntityName: nameof(Patient),
                EntityId: patient.ExternalId.ToString()),
            cancellationToken);

        return MapToDto(patient);
    }

    public async Task<bool> UpdateAsync(
        Guid id,
        UpdatePatientRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var tenantId = RequireTenant();

        var patient =
            await _patientRepository.GetByExternalIdAsync(
                id,
                cancellationToken);

        if (patient is null)
        {
            return false;
        }

        var patientId = NormalizeRequired(
            request.PatientId,
            nameof(request.PatientId),
            "Patient ID is required.");

        var mrn = NormalizeRequired(
            request.Mrn,
            nameof(request.Mrn),
            "MRN is required.");

        var accountNumber = NormalizeRequired(
            request.AccountNumber,
            nameof(request.AccountNumber),
            "Account number is required.");

        var firstName = NormalizeRequired(
            request.FirstName,
            nameof(request.FirstName),
            "Patient first name is required.");

        var lastName = NormalizeRequired(
            request.LastName,
            nameof(request.LastName),
            "Patient last name is required.");

        ValidateDateOfBirth(request.DateOfBirth);

        if (await _patientRepository.ExistsByPatientIdAsync(
                patientId,
                id,
                cancellationToken))
        {
            throw new InvalidOperationException(
                $"A patient with ID '{patientId}' already exists.");
        }

        if (await _patientRepository.ExistsByMrnAsync(
                mrn,
                id,
                cancellationToken))
        {
            throw new InvalidOperationException(
                $"A patient with MRN '{mrn}' already exists.");
        }

        if (await _patientRepository.ExistsByAccountNumberAsync(
                accountNumber,
                id,
                cancellationToken))
        {
            throw new InvalidOperationException(
                $"A patient with account number '{accountNumber}' already exists.");
        }

        var gender = ParseEnum<PatientGender>(
            request.Gender,
            nameof(request.Gender));

        var patientClass = ParseEnum<PatientClass>(
            request.PatientClass,
            nameof(request.PatientClass));

        var referralSource =
            ParseNullableEnum<ReferralSource>(
                request.ReferralSource,
                nameof(request.ReferralSource));

        patient.UpdateIdentifiers(
            patientId,
            mrn,
            accountNumber);

        patient.UpdateDemographics(
            firstName,
            request.MiddleName,
            lastName,
            request.PreferredName,
            request.DateOfBirth,
            gender);

        patient.UpdatePatientInformation(
            request.RegistrationDate,
            request.LastVisitDate,
            patientClass,
            request.PreferredPharmacy,
            referralSource,
            request.Notes);

        await _patientRepository.UpdateAsync(
            patient,
            cancellationToken);

        await _patientRepository.SaveChangesAsync(
            cancellationToken);

        await _auditService.WriteAsync(
            new AuditEntry(
                TenantId: tenantId,
                UserId: null,
                Action: "PatientUpdated",
                EntityName: nameof(Patient),
                EntityId: patient.ExternalId.ToString()),
            cancellationToken);

        return true;
    }

    public Task<bool> DeactivateAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return SetStatusAsync(
            id,
            active: false,
            cancellationToken);
    }

    public Task<bool> ActivateAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return SetStatusAsync(
            id,
            active: true,
            cancellationToken);
    }

    public async Task<bool> SetPortalAccessAsync(
        Guid id,
        string status,
        CancellationToken cancellationToken = default)
    {
        var tenantId = RequireTenant();

        var patient =
            await _patientRepository.GetByExternalIdAsync(
                id,
                cancellationToken);

        if (patient is null)
        {
            return false;
        }

        if (!Enum.TryParse<PortalAccessStatus>(
                status,
                true,
                out var portalStatus))
        {
            throw new ArgumentException(
                "Invalid portal access status.",
                nameof(status));
        }

        switch (portalStatus)
        {
            case PortalAccessStatus.Enabled:
                patient.EnablePortalAccess();
                break;

            case PortalAccessStatus.Disabled:
                patient.DisablePortalAccess();
                break;

            case PortalAccessStatus.Pending:
                patient.SetPortalAccessPending();
                break;

            default:
                throw new ArgumentException(
                    "Invalid portal access status.",
                    nameof(status));
        }

        await _patientRepository.UpdateAsync(
            patient,
            cancellationToken);

        await _patientRepository.SaveChangesAsync(
            cancellationToken);

        await _auditService.WriteAsync(
            new AuditEntry(
                TenantId: tenantId,
                UserId: null,
                Action: "PatientPortalAccessChanged",
                EntityName: nameof(Patient),
                EntityId: patient.ExternalId.ToString()),
            cancellationToken);

        return true;
    }

    public async Task<bool> SetCommunicationConsentAsync(
        Guid id,
        string status,
        CancellationToken cancellationToken = default)
    {
        var tenantId = RequireTenant();

        var patient =
            await _patientRepository.GetByExternalIdAsync(
                id,
                cancellationToken);

        if (patient is null)
        {
            return false;
        }

        if (!Enum.TryParse<CommunicationConsentStatus>(
                status,
                true,
                out var consentStatus))
        {
            throw new ArgumentException(
                "Invalid communication consent status.",
                nameof(status));
        }

        switch (consentStatus)
        {
            case CommunicationConsentStatus.Granted:
                patient.GrantCommunicationConsent();
                break;

            case CommunicationConsentStatus.Declined:
                patient.DeclineCommunicationConsent();
                break;

            case CommunicationConsentStatus.Pending:
                patient.SetCommunicationConsentPending();
                break;

            default:
                throw new ArgumentException(
                    "Invalid communication consent status.",
                    nameof(status));
        }

        await _patientRepository.UpdateAsync(
            patient,
            cancellationToken);

        await _patientRepository.SaveChangesAsync(
            cancellationToken);

        await _auditService.WriteAsync(
            new AuditEntry(
                TenantId: tenantId,
                UserId: null,
                Action: "PatientCommunicationConsentChanged",
                EntityName: nameof(Patient),
                EntityId: patient.ExternalId.ToString()),
            cancellationToken);

        return true;
    }

    private async Task<bool> SetStatusAsync(
        Guid id,
        bool active,
        CancellationToken cancellationToken)
    {
        var tenantId = RequireTenant();

        var patient =
            await _patientRepository.GetByExternalIdAsync(
                id,
                cancellationToken);

        if (patient is null)
        {
            return false;
        }

        if (active)
        {
            patient.Activate();
        }
        else
        {
            patient.Deactivate();
        }

        await _patientRepository.UpdateAsync(
            patient,
            cancellationToken);

        await _patientRepository.SaveChangesAsync(
            cancellationToken);

        await _auditService.WriteAsync(
            new AuditEntry(
                TenantId: tenantId,
                UserId: null,
                Action: active
                    ? "PatientActivated"
                    : "PatientDeactivated",
                EntityName: nameof(Patient),
                EntityId: patient.ExternalId.ToString()),
            cancellationToken);

        return true;
    }

    private long RequireTenant()
    {
        if (!_tenantContext.TenantId.HasValue)
        {
            throw new InvalidOperationException(
                "A tenant context is required for this operation.");
        }

        return _tenantContext.TenantId.Value;
    }

    private static string NormalizeRequired(
        string? value,
        string parameterName,
        string errorMessage)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException(
                errorMessage,
                parameterName);
        }

        return value.Trim();
    }

    private static void ValidateDateOfBirth(
        DateTime dateOfBirth)
    {
        if (dateOfBirth == default)
        {
            throw new ArgumentException(
                "Date of birth is required.",
                nameof(dateOfBirth));
        }

        if (dateOfBirth.Date > DateTime.UtcNow.Date)
        {
            throw new ArgumentException(
                "Date of birth cannot be in the future.",
                nameof(dateOfBirth));
        }
    }

    private static T ParseEnum<T>(
        string? value,
        string parameterName)
        where T : struct, Enum
    {
        if (string.IsNullOrWhiteSpace(value) ||
            !Enum.TryParse<T>(
                value,
                true,
                out var result))
        {
            throw new ArgumentException(
                $"Invalid {typeof(T).Name}.",
                parameterName);
        }

        return result;
    }

    private static T? ParseNullableEnum<T>(
        string? value,
        string parameterName)
        where T : struct, Enum
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        return ParseEnum<T>(
            value,
            parameterName);
    }

    private static PatientDto MapToDto(
        Patient patient)
    {
        return new PatientDto(
            patient.ExternalId,
            patient.TenantId,
            patient.PatientId,
            patient.Mrn,
            patient.AccountNumber,
            patient.FirstName,
            patient.MiddleName,
            patient.LastName,
            patient.PreferredName,
            patient.DateOfBirth,
            patient.Gender.ToString(),
            patient.Status.ToString(),
            patient.RegistrationDate,
            patient.LastVisitDate,
            patient.PatientClass.ToString(),
            patient.PortalAccess.ToString(),
            patient.CommunicationConsent.ToString(),
            patient.PreferredPharmacy,
            patient.ReferralSource?.ToString(),
            patient.FinancialAssistance.ToString(),
            patient.Notes,
            patient.CreatedAtUtc,
            patient.UpdatedAtUtc);
    }
}