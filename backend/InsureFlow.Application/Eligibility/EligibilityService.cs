using InsureFlow.Application.Persistence;
using InsureFlow.Application.Tenants;
using InsureFlow.Contracts.Eligibility;
using InsureFlow.Domain.Entities;
using InsureFlow.Domain.Enums;

namespace InsureFlow.Application.Eligibility;

public sealed class EligibilityService(
    IEligibilityRequestRepository requestRepository,
    IEligibilityResponseRepository responseRepository,
    IEligibilityBenefitRepository benefitRepository,
    IEligibilityHistoryRepository historyRepository,
    ITenantContext tenantContext)
    : IEligibilityService
{
    public async Task<EligibilityRequestDto> CreateAsync(
        CreateEligibilityRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var tenantId = RequireTenant();

        ValidatePositiveId(
            request.PatientId,
            nameof(request.PatientId));

        ValidatePositiveId(
            request.PatientInsuranceId,
            nameof(request.PatientInsuranceId));

        ValidatePositiveId(
            request.PayerId,
            nameof(request.PayerId));

        var requestType =
            ParseEnum<EligibilityRequestType>(
                request.RequestType,
                nameof(request.RequestType));

        var source =
            ParseEnum<EligibilityRequestSource>(
                request.Source,
                nameof(request.Source));

        var requestNumber =
            string.IsNullOrWhiteSpace(request.RequestNumber)
                ? GenerateRequestNumber()
                : request.RequestNumber.Trim();

        if (requestNumber.Length > 50)
        {
            throw new ArgumentException(
                "Request number cannot exceed 50 characters.",
                nameof(request));
        }

        if (await requestRepository
                .ExistsByRequestNumberAsync(
                    requestNumber,
                    cancellationToken))
        {
            throw new InvalidOperationException(
                $"Eligibility request number '{requestNumber}' already exists.");
        }

        var now = DateTime.UtcNow;

        var entity = new EligibilityRequest(
            tenantId,
            request.PatientId,
            request.PatientInsuranceId,
            request.PayerId,
            requestNumber,
            requestType,
            source,
            now);

        entity.SetTraceId(request.TraceId);
        entity.SetRawRequest(request.RawRequest);

        await requestRepository.AddAsync(
            entity,
            cancellationToken);

        await historyRepository.AddAsync(
            new EligibilityHistory(
                tenantId,
                entity.Id,
                EligibilityRequestStatus.Pending.ToString(),
                EligibilityRequestStatus.Pending.ToString(),
                "Eligibility request created.",
                now,
                null),
            cancellationToken);

        await requestRepository.SaveChangesAsync(
            cancellationToken);

        return MapRequest(entity);
    }

    public async Task<EligibilityRequestDto?> GetByIdAsync(
        long id,
        bool includeDetails = true,
        CancellationToken cancellationToken = default)
    {
        ValidatePositiveId(
            id,
            nameof(id));

        var entity =
            await requestRepository.GetByIdAsync(
                id,
                includeDetails,
                cancellationToken);

        return entity is null
            ? null
            : MapRequest(entity);
    }

    public async Task<EligibilityRequestDto?> GetByExternalIdAsync(
        Guid externalId,
        bool includeDetails = true,
        CancellationToken cancellationToken = default)
    {
        if (externalId == Guid.Empty)
        {
            throw new ArgumentException(
                "External ID is required.",
                nameof(externalId));
        }

        var entity =
            await requestRepository.GetByExternalIdAsync(
                externalId,
                cancellationToken);

        if (entity is null)
        {
            return null;
        }

        if (!includeDetails)
        {
            return MapRequest(entity);
        }

        var detailedEntity =
            await requestRepository.GetByIdAsync(
                entity.Id,
                true,
                cancellationToken);

        return detailedEntity is null
            ? null
            : MapRequest(detailedEntity);
    }

    public async Task<EligibilityListResponse> GetPagedAsync(
        int pageNumber,
        int pageSize,
        string? status = null,
        long? patientId = null,
        long? patientInsuranceId = null,
        long? payerId = null,
        CancellationToken cancellationToken = default)
    {
        ValidatePage(
            pageNumber,
            pageSize);

        if (patientId.HasValue)
        {
            ValidatePositiveId(
                patientId.Value,
                nameof(patientId));
        }

        if (patientInsuranceId.HasValue)
        {
            ValidatePositiveId(
                patientInsuranceId.Value,
                nameof(patientInsuranceId));
        }

        if (payerId.HasValue)
        {
            ValidatePositiveId(
                payerId.Value,
                nameof(payerId));
        }

        EligibilityRequestStatus? parsedStatus = null;

        if (!string.IsNullOrWhiteSpace(status))
        {
            parsedStatus =
                ParseEnum<EligibilityRequestStatus>(
                    status,
                    nameof(status));
        }

        var items =
            await requestRepository.GetPagedAsync(
                pageNumber,
                pageSize,
                parsedStatus,
                patientId,
                patientInsuranceId,
                payerId,
                cancellationToken);

        var totalCount =
            await requestRepository.CountAsync(
                parsedStatus,
                patientId,
                patientInsuranceId,
                payerId,
                cancellationToken);

        return new EligibilityListResponse
        {
            Items = items
                .Select(MapRequest)
                .ToList(),

            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalCount = totalCount
        };
    }

    public async Task<EligibilityRequestDto> MarkProcessingAsync(
        long id,
        CancellationToken cancellationToken = default)
    {
        var entity =
            await GetRequiredRequestAsync(
                id,
                false,
                cancellationToken);

        var previousStatus =
            entity.Status;

        if (previousStatus !=
            EligibilityRequestStatus.Pending)
        {
            throw new InvalidOperationException(
                $"Eligibility request cannot be moved to Processing " +
                $"from {previousStatus}.");
        }

        entity.MarkProcessing();

        await historyRepository.AddAsync(
            new EligibilityHistory(
                entity.TenantId,
                entity.Id,
                previousStatus.ToString(),
                EligibilityRequestStatus.Processing.ToString(),
                "Eligibility request processing started.",
                DateTime.UtcNow,
                null),
            cancellationToken);

        await requestRepository.SaveChangesAsync(
            cancellationToken);

        return MapRequest(entity);
    }

    public async Task<EligibilityRequestDto> RecordResponseAsync(
        long id,
        RecordEligibilityResponseRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var entity =
            await GetRequiredRequestAsync(
                id,
                false,
                cancellationToken);

        if (entity.Status !=
                EligibilityRequestStatus.Pending &&
            entity.Status !=
                EligibilityRequestStatus.Processing)
        {
            throw new InvalidOperationException(
                $"An eligibility response cannot be recorded " +
                $"for a request in {entity.Status} status.");
        }

        var responseStatus =
            ParseEnum<EligibilityResponseStatus>(
                request.Status,
                nameof(request.Status));

        CoverageStatus? coverageStatus = null;

        if (!string.IsNullOrWhiteSpace(
                request.CoverageStatus))
        {
            coverageStatus =
                ParseEnum<CoverageStatus>(
                    request.CoverageStatus,
                    nameof(request.CoverageStatus));
        }

        ValidateFinancialValues(request);

        var now = DateTime.UtcNow;

        var response =
            await responseRepository.GetByRequestIdAsync(
                entity.Id,
                true,
                cancellationToken);

        if (response is null)
        {
            response = new EligibilityResponse(
                entity.TenantId,
                entity.Id,
                responseStatus,
                now);

            await responseRepository.AddAsync(
                response,
                cancellationToken);
        }

        response.SetCoverage(
            request.PayerReference,
            coverageStatus,
            request.EffectiveDate,
            request.TerminationDate);

        response.SetFinancialDetails(
            request.DeductibleIndividual,
            request.DeductibleRemaining,
            request.Copay,
            request.CoinsurancePercent);

        response.SetResponseMessage(
            request.ResponseMessage);

        response.SetRawResponse(
            request.RawResponse);

        ApplyResponseStatus(
            response,
            responseStatus,
            now);

        foreach (var benefitRequest in request.Benefits)
        {
            ValidateBenefit(
                benefitRequest);

            var benefit =
                new EligibilityBenefit(
                    entity.TenantId,
                    response.Id,
                    benefitRequest.Category,
                    benefitRequest.ServiceTypeCode,
                    benefitRequest.Description,
                    benefitRequest.CoverageLevel,
                    benefitRequest.Copay,
                    benefitRequest.CoinsurancePercent,
                    benefitRequest.Deductible,
                    benefitRequest.Remaining);

            if (ParseEnum<EligibilityBenefitStatus>(
                    benefitRequest.Status,
                    nameof(benefitRequest.Status))
                == EligibilityBenefitStatus.Inactive)
            {
                benefit.Deactivate();
            }

            await benefitRepository.AddAsync(
                benefit,
                cancellationToken);
        }

        var previousStatus =
            entity.Status;

        entity.MarkCompleted(now);

        await historyRepository.AddAsync(
            new EligibilityHistory(
                entity.TenantId,
                entity.Id,
                previousStatus.ToString(),
                EligibilityRequestStatus.Completed.ToString(),
                BuildResponseHistoryReason(
                    responseStatus),
                now,
                null),
            cancellationToken);

        await requestRepository.SaveChangesAsync(
            cancellationToken);

        var savedEntity =
            await requestRepository.GetByIdAsync(
                entity.Id,
                true,
                cancellationToken);

        return savedEntity is null
            ? throw new InvalidOperationException(
                "Eligibility request could not be reloaded after saving.")
            : MapRequest(savedEntity);
    }

    public async Task<EligibilityRequestDto> MarkFailedAsync(
        long id,
        string? errorCode,
        string? errorMessage,
        CancellationToken cancellationToken = default)
    {
        var entity =
            await GetRequiredRequestAsync(
                id,
                false,
                cancellationToken);

        if (entity.Status ==
                EligibilityRequestStatus.Completed ||
            entity.Status ==
                EligibilityRequestStatus.Cancelled)
        {
            throw new InvalidOperationException(
                $"Eligibility request cannot be failed from {entity.Status} status.");
        }

        var previousStatus =
            entity.Status;

        var now = DateTime.UtcNow;

        entity.MarkFailed(
            errorCode,
            errorMessage,
            now);

        await historyRepository.AddAsync(
            new EligibilityHistory(
                entity.TenantId,
                entity.Id,
                previousStatus.ToString(),
                EligibilityRequestStatus.Failed.ToString(),
                errorMessage,
                now,
                null),
            cancellationToken);

        await requestRepository.SaveChangesAsync(
            cancellationToken);

        return MapRequest(entity);
    }

    public async Task<EligibilityRequestDto> CancelAsync(
        long id,
        CancellationToken cancellationToken = default)
    {
        var entity =
            await GetRequiredRequestAsync(
                id,
                false,
                cancellationToken);

        if (entity.Status ==
                EligibilityRequestStatus.Completed ||
            entity.Status ==
                EligibilityRequestStatus.Failed ||
            entity.Status ==
                EligibilityRequestStatus.Cancelled)
        {
            throw new InvalidOperationException(
                $"Eligibility request cannot be cancelled from {entity.Status} status.");
        }

        var previousStatus =
            entity.Status;

        var now = DateTime.UtcNow;

        entity.Cancel();

        await historyRepository.AddAsync(
            new EligibilityHistory(
                entity.TenantId,
                entity.Id,
                previousStatus.ToString(),
                EligibilityRequestStatus.Cancelled.ToString(),
                "Eligibility request cancelled.",
                now,
                null),
            cancellationToken);

        await requestRepository.SaveChangesAsync(
            cancellationToken);

        return MapRequest(entity);
    }

    public async Task<IReadOnlyList<EligibilityHistoryDto>>
        GetHistoryAsync(
            long requestId,
            CancellationToken cancellationToken = default)
    {
        ValidatePositiveId(
            requestId,
            nameof(requestId));

        await GetRequiredRequestAsync(
            requestId,
            false,
            cancellationToken);

        var history =
            await historyRepository.GetByRequestIdAsync(
                requestId,
                cancellationToken);

        return history
            .Select(MapHistory)
            .ToList();
    }

    public async Task<EligibilityRequestDto> UpdateAsync(
        long id,
        UpdateEligibilityRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var entity =
            await GetRequiredRequestAsync(
                id,
                false,
                cancellationToken);

        if (entity.Status !=
            EligibilityRequestStatus.Pending)
        {
            throw new InvalidOperationException(
                "Only pending eligibility requests can be updated.");
        }

        var now = DateTime.UtcNow;

        entity.SetTraceId(
            request.TraceId);

        entity.SetRawRequest(
            request.RawRequest);

        await historyRepository.AddAsync(
            new EligibilityHistory(
                entity.TenantId,
                entity.Id,
                entity.Status.ToString(),
                entity.Status.ToString(),
                "Eligibility request metadata updated.",
                now,
                null),
            cancellationToken);

        await requestRepository.SaveChangesAsync(
            cancellationToken);

        return MapRequest(entity);
    }

    public Task<EligibilityRequestDto> SubmitAsync(
        long id,
        CancellationToken cancellationToken = default)
    {
        return MarkProcessingAsync(
            id,
            cancellationToken);
    }

    public async Task<EligibilityRequestDto> RetryAsync(
        long id,
        CancellationToken cancellationToken = default)
    {
        var entity =
            await GetRequiredRequestAsync(
                id,
                false,
                cancellationToken);

        if (entity.Status !=
            EligibilityRequestStatus.Failed)
        {
            throw new InvalidOperationException(
                "Only failed eligibility requests can be retried.");
        }

        var now = DateTime.UtcNow;

        entity.Retry();

        await historyRepository.AddAsync(
            new EligibilityHistory(
                entity.TenantId,
                entity.Id,
                EligibilityRequestStatus.Failed.ToString(),
                EligibilityRequestStatus.Pending.ToString(),
                "Failed eligibility request queued for retry.",
                now,
                null),
            cancellationToken);

        await requestRepository.SaveChangesAsync(
            cancellationToken);

        return MapRequest(entity);
    }

    public async Task<EligibilityResponseDto?> GetResponseAsync(
        long requestId,
        CancellationToken cancellationToken = default)
    {
        await GetRequiredRequestAsync(
            requestId,
            false,
            cancellationToken);

        var response =
            await responseRepository.GetByRequestIdAsync(
                requestId,
                true,
                cancellationToken);

        return response is null
            ? null
            : MapResponse(response);
    }

    public async Task<IReadOnlyList<EligibilityBenefitDto>>
        GetBenefitsAsync(
            long requestId,
            CancellationToken cancellationToken = default)
    {
        await GetRequiredRequestAsync(
            requestId,
            false,
            cancellationToken);

        var response =
            await responseRepository.GetByRequestIdAsync(
                requestId,
                false,
                cancellationToken);

        if (response is null)
        {
            return Array.Empty<EligibilityBenefitDto>();
        }

        var benefits =
            await benefitRepository.GetByResponseIdAsync(
                response.Id,
                false,
                cancellationToken);

        return benefits
            .Select(MapBenefit)
            .ToList();
    }

    public async Task<EligibilityBenefitDto> AddBenefitAsync(
        long requestId,
        CreateEligibilityBenefitRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var entity =
            await GetRequiredRequestAsync(
                requestId,
                false,
                cancellationToken);

        if (entity.Status !=
            EligibilityRequestStatus.Completed)
        {
            throw new InvalidOperationException(
                "Benefits can only be added to completed eligibility requests.");
        }

        var response =
            await responseRepository.GetByRequestIdAsync(
                requestId,
                false,
                cancellationToken);

        if (response is null)
        {
            throw new InvalidOperationException(
                "An eligibility response must exist before adding benefits.");
        }

        ValidateBenefit(
            request);

        var benefitStatus =
            ParseEnum<EligibilityBenefitStatus>(
                request.Status,
                nameof(request.Status));

        var benefit =
            new EligibilityBenefit(
                entity.TenantId,
                response.Id,
                request.Category,
                request.ServiceTypeCode,
                request.Description,
                request.CoverageLevel,
                request.Copay,
                request.CoinsurancePercent,
                request.Deductible,
                request.Remaining);

        if (benefitStatus ==
            EligibilityBenefitStatus.Inactive)
        {
            benefit.Deactivate();
        }

        await benefitRepository.AddAsync(
            benefit,
            cancellationToken);

        await benefitRepository.SaveChangesAsync(
            cancellationToken);

        return MapBenefit(
            benefit);
    }

    public async Task<IReadOnlyList<EligibilityHistoryDto>>
        GetAllHistoryAsync(
            int pageNumber,
            int pageSize,
            CancellationToken cancellationToken = default)
    {
        ValidatePage(
            pageNumber,
            pageSize);

        var history =
            await historyRepository.GetPagedAsync(
                pageNumber,
                pageSize,
                cancellationToken);

        return history
            .Select(MapHistory)
            .ToList();
    }

    public Task<EligibilityListResponse> GetFailuresAsync(
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        return GetPagedAsync(
            pageNumber,
            pageSize,
            status: EligibilityRequestStatus.Failed.ToString(),
            cancellationToken: cancellationToken);
    }

    public Task<EligibilityListResponse> GetByPatientAsync(
        long patientId,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        ValidatePositiveId(
            patientId,
            nameof(patientId));

        return GetPagedAsync(
            pageNumber,
            pageSize,
            patientId: patientId,
            cancellationToken: cancellationToken);
    }

    public Task<EligibilityListResponse> GetByInsuranceAsync(
        long patientInsuranceId,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        ValidatePositiveId(
            patientInsuranceId,
            nameof(patientInsuranceId));

        return GetPagedAsync(
            pageNumber,
            pageSize,
            patientInsuranceId: patientInsuranceId,
            cancellationToken: cancellationToken);
    }

    private async Task<EligibilityRequest>
        GetRequiredRequestAsync(
            long id,
            bool includeDetails,
            CancellationToken cancellationToken)
    {
        ValidatePositiveId(
            id,
            nameof(id));

        var entity =
            await requestRepository.GetByIdAsync(
                id,
                includeDetails,
                cancellationToken);

        return entity
            ?? throw new KeyNotFoundException(
                $"Eligibility request with ID {id} was not found.");
    }

    private long RequireTenant()
    {
        if (!tenantContext.TenantId.HasValue ||
            tenantContext.TenantId.Value <= 0)
        {
            throw new InvalidOperationException(
                "A valid tenant context is required.");
        }

        return tenantContext.TenantId.Value;
    }

    private static void ValidatePositiveId(
        long value,
        string parameterName)
    {
        if (value <= 0)
        {
            throw new ArgumentException(
                "ID must be greater than zero.",
                parameterName);
        }
    }

    private static void ValidatePage(
        int pageNumber,
        int pageSize)
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

        if (pageSize > 200)
        {
            throw new ArgumentOutOfRangeException(
                nameof(pageSize),
                "Page size cannot exceed 200.");
        }
    }

    private static string GenerateRequestNumber()
    {
        return
            $"ELG-{DateTime.UtcNow:yyyyMMddHHmmssfff}-" +
            $"{Guid.NewGuid():N}"[..8].ToUpperInvariant();
    }

    private static TEnum ParseEnum<TEnum>(
        string? value,
        string parameterName)
        where TEnum : struct, Enum
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException(
                $"{parameterName} is required.",
                parameterName);
        }

        if (Enum.TryParse<TEnum>(
                value.Trim(),
                true,
                out var result) &&
            Enum.IsDefined(result))
        {
            return result;
        }

        throw new ArgumentException(
            $"Invalid {parameterName} value '{value}'.",
            parameterName);
    }

    private static void ValidateFinancialValues(
        RecordEligibilityResponseRequest request)
    {
        ValidateNonNegative(
            request.DeductibleIndividual,
            nameof(request.DeductibleIndividual));

        ValidateNonNegative(
            request.DeductibleRemaining,
            nameof(request.DeductibleRemaining));

        ValidateNonNegative(
            request.Copay,
            nameof(request.Copay));

        ValidatePercentage(
            request.CoinsurancePercent,
            nameof(request.CoinsurancePercent));

        if (request.TerminationDate.HasValue &&
            request.EffectiveDate.HasValue &&
            request.TerminationDate.Value <
            request.EffectiveDate.Value)
        {
            throw new ArgumentException(
                "Termination date cannot be before effective date.");
        }
    }

    private static void ValidateBenefit(
        CreateEligibilityBenefitRequest benefit)
    {
        ArgumentNullException.ThrowIfNull(
            benefit);

        if (string.IsNullOrWhiteSpace(
                benefit.Category))
        {
            throw new ArgumentException(
                "Benefit category is required.");
        }

        if (benefit.Category.Length > 100)
        {
            throw new ArgumentException(
                "Benefit category cannot exceed 100 characters.");
        }

        if (benefit.ServiceTypeCode?.Length > 20)
        {
            throw new ArgumentException(
                "Service type code cannot exceed 20 characters.");
        }

        if (benefit.Description?.Length > 255)
        {
            throw new ArgumentException(
                "Benefit description cannot exceed 255 characters.");
        }

        if (benefit.CoverageLevel?.Length > 50)
        {
            throw new ArgumentException(
                "Coverage level cannot exceed 50 characters.");
        }

        ValidateNonNegative(
            benefit.Copay,
            nameof(benefit.Copay));

        ValidateNonNegative(
            benefit.Deductible,
            nameof(benefit.Deductible));

        ValidateNonNegative(
            benefit.Remaining,
            nameof(benefit.Remaining));

        ValidatePercentage(
            benefit.CoinsurancePercent,
            nameof(benefit.CoinsurancePercent));

        ParseEnum<EligibilityBenefitStatus>(
            benefit.Status,
            nameof(benefit.Status));
    }

    private static void ValidateNonNegative(
        decimal? value,
        string parameterName)
    {
        if (value.HasValue &&
            value.Value < 0)
        {
            throw new ArgumentException(
                $"{parameterName} cannot be negative.",
                parameterName);
        }
    }

    private static void ValidatePercentage(
        decimal? value,
        string parameterName)
    {
        if (value.HasValue &&
            (value.Value < 0 ||
             value.Value > 100))
        {
            throw new ArgumentException(
                $"{parameterName} must be between 0 and 100.",
                parameterName);
        }
    }

    private static void ApplyResponseStatus(
        EligibilityResponse response,
        EligibilityResponseStatus status,
        DateTime respondedAtUtc)
    {
        switch (status)
        {
            case EligibilityResponseStatus.Verified:

                response.MarkVerified(
                    respondedAtUtc);

                break;

            case EligibilityResponseStatus.Inactive:

                response.MarkInactive(
                    respondedAtUtc);

                break;

            case EligibilityResponseStatus.Error:

                response.MarkError(
                    response.ResponseMessage,
                    respondedAtUtc);

                break;

            case EligibilityResponseStatus.NotVerified:

                response.SetResponseMessage(
                    response.ResponseMessage);

                break;

            default:

                throw new ArgumentOutOfRangeException(
                    nameof(status),
                    status,
                    "Unsupported eligibility response status.");
        }
    }

    private static string BuildResponseHistoryReason(
        EligibilityResponseStatus status)
    {
        return status switch
        {
            EligibilityResponseStatus.Verified =>
                "Eligibility response verified.",

            EligibilityResponseStatus.Inactive =>
                "Eligibility response indicates inactive coverage.",

            EligibilityResponseStatus.NotVerified =>
                "Eligibility response could not verify coverage.",

            EligibilityResponseStatus.Error =>
                "Eligibility response returned an error.",

            _ =>
                "Eligibility response recorded."
        };
    }

    private static EligibilityRequestDto MapRequest(
        EligibilityRequest entity)
    {
        return new EligibilityRequestDto
        {
            Id =
                entity.Id,

            ExternalId =
                entity.ExternalId,

            TenantId =
                entity.TenantId,

            PatientId =
                entity.PatientId,

            PatientInsuranceId =
                entity.PatientInsuranceId,

            PayerId =
                entity.PayerId,

            RequestNumber =
                entity.RequestNumber,

            RequestType =
                entity.RequestType.ToString(),

            Status =
                entity.Status.ToString(),

            Source =
                entity.Source.ToString(),

            RequestedAt =
                entity.RequestedAt,

            CompletedAt =
                entity.CompletedAt,

            ErrorCode =
                entity.ErrorCode,

            ErrorMessage =
                entity.ErrorMessage,

            TraceId =
                entity.TraceId,

            Response =
                entity.Response is null
                    ? null
                    : MapResponse(entity.Response),

            History =
                entity.History
                    .Select(MapHistory)
                    .ToList()
        };
    }

    private static EligibilityResponseDto MapResponse(
        EligibilityResponse entity)
    {
        return new EligibilityResponseDto
        {
            Id =
                entity.Id,

            ExternalId =
                entity.ExternalId,

            TenantId =
                entity.TenantId,

            EligibilityRequestId =
                entity.EligibilityRequestId,

            Status =
                entity.Status.ToString(),

            PayerReference =
                entity.PayerReference,

            CoverageStatus =
                entity.CoverageStatus?.ToString(),

            EffectiveDate =
                entity.EffectiveDate,

            TerminationDate =
                entity.TerminationDate,

            DeductibleIndividual =
                entity.DeductibleIndividual,

            DeductibleRemaining =
                entity.DeductibleRemaining,

            Copay =
                entity.Copay,

            CoinsurancePercent =
                entity.CoinsurancePercent,

            ResponseMessage =
                entity.ResponseMessage,

            RespondedAt =
                entity.RespondedAt,

            Benefits =
                entity.Benefits
                    .Select(MapBenefit)
                    .ToList()
        };
    }

    private static EligibilityBenefitDto MapBenefit(
        EligibilityBenefit entity)
    {
        return new EligibilityBenefitDto
        {
            Id =
                entity.Id,

            ExternalId =
                entity.ExternalId,

            TenantId =
                entity.TenantId,

            EligibilityResponseId =
                entity.EligibilityResponseId,

            Category =
                entity.Category,

            ServiceTypeCode =
                entity.ServiceTypeCode,

            Description =
                entity.Description,

            CoverageLevel =
                entity.CoverageLevel,

            Copay =
                entity.Copay,

            CoinsurancePercent =
                entity.CoinsurancePercent,

            Deductible =
                entity.Deductible,

            Remaining =
                entity.Remaining,

            Status =
                entity.Status.ToString()
        };
    }

    private static EligibilityHistoryDto MapHistory(
        EligibilityHistory entity)
    {
        return new EligibilityHistoryDto
        {
            Id =
                entity.Id,

            ExternalId =
                entity.ExternalId,

            TenantId =
                entity.TenantId,

            EligibilityRequestId =
                entity.EligibilityRequestId,

            FromStatus =
                entity.FromStatus,

            ToStatus =
                entity.ToStatus,

            Reason =
                entity.Reason,

            ChangedAt =
                entity.ChangedAt,

            ChangedByUserId =
                entity.ChangedByUserId
        };
    }
} 