
using InsureFlow.Application.Persistence;
using InsureFlow.Application.Tenants;
using InsureFlow.Contracts.Denials;
using InsureFlow.Domain.Entities;

namespace InsureFlow.Application.Denials;

public sealed class DenialService : IDenialService
{
    private readonly IDenialRepository _denialRepository;
    private readonly IClaimRepository _claimRepository;
    private readonly ITenantContext _tenantContext;

    public DenialService(
        IDenialRepository denialRepository,
        IClaimRepository claimRepository,
        ITenantContext tenantContext)
    {
        _denialRepository = denialRepository;
        _claimRepository = claimRepository;
        _tenantContext = tenantContext;
    }

    public async Task<IReadOnlyList<DenialResponse>> GetPagedAsync(
        string? search,
        string? status,
        string? denialCategory,
        Guid? claimExternalId,
        DateTime? denialDateFrom,
        DateTime? denialDateTo,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        RequireTenant();

        if (page < 1)
            throw new ArgumentException("Page must be greater than zero.");

        if (pageSize < 1 || pageSize > 100)
            throw new ArgumentException("Page size must be between 1 and 100.");

        long? claimId = null;

        if (claimExternalId.HasValue)
        {
            var claim = await _claimRepository.GetByExternalIdAsync(
                claimExternalId.Value,
                cancellationToken);

            if (claim is null)
                return Array.Empty<DenialResponse>();

            claimId = claim.Id;
        }

        var denials = await _denialRepository.GetPagedAsync(
            search,
            status,
            denialCategory,
            claimId,
            denialDateFrom,
            denialDateTo,
            page,
            pageSize,
            cancellationToken);

        var responses = new List<DenialResponse>(denials.Count);

        foreach (var denial in denials)
        {
            responses.Add(await MapAsync(denial, cancellationToken));
        }

        return responses;
    }

    public async Task<DenialResponse?> GetByExternalIdAsync(
        Guid externalId,
        CancellationToken cancellationToken = default)
    {
        RequireTenant();

        var denial = await _denialRepository.GetByExternalIdAsync(
            externalId,
            cancellationToken);

        if (denial is null)
            return null;

        return await MapAsync(denial, cancellationToken);
    }

    public async Task<DenialResponse> CreateAsync(
        CreateDenialRequest request,
        CancellationToken cancellationToken = default)
    {
        var tenantId = RequireTenant();

        var claim = await _claimRepository.GetByExternalIdAsync(
            request.ClaimExternalId,
            cancellationToken);

        if (claim is null || claim.TenantId != tenantId)
            throw new ArgumentException("The specified claim was not found.");

        var denialNumber = request.DenialNumber?.Trim();

        if (string.IsNullOrWhiteSpace(denialNumber))
            throw new ArgumentException("Denial number is required.");

        if (await _denialRepository.ExistsByDenialNumberAsync(
                denialNumber,
                cancellationToken))
        {
            throw new InvalidOperationException(
                "A denial with this denial number already exists.");
        }

        var denial = new Denial(
            tenantId,
            claim.Id,
            denialNumber,
            request.DenialCode,
            request.DenialReason,
            request.DenialCategory,
            request.DeniedAmount,
            request.DenialDate,
            request.AppealDeadline);

        await _denialRepository.AddAsync(denial, cancellationToken);
        await _denialRepository.SaveChangesAsync(cancellationToken);

        return await MapAsync(denial, cancellationToken);
    }

    public async Task<DenialResponse?> UpdateAsync(
        Guid externalId,
        UpdateDenialRequest request,
        CancellationToken cancellationToken = default)
    {
        RequireTenant();

        var denial = await _denialRepository.GetByExternalIdAsync(
            externalId,
            cancellationToken);

        if (denial is null)
            return null;

        denial.UpdateDetails(
            request.DenialCode,
            request.DenialReason,
            request.DenialCategory,
            request.DeniedAmount,
            request.DenialDate,
            request.AppealDeadline);

        _denialRepository.Update(denial);
        await _denialRepository.SaveChangesAsync(cancellationToken);

        return await MapAsync(denial, cancellationToken);
    }

    public async Task<IReadOnlyList<DenialActivityResponse>> GetActivitiesAsync(
        Guid denialExternalId,
        CancellationToken cancellationToken = default)
    {
        RequireTenant();

        var denial = await _denialRepository.GetByExternalIdAsync(
            denialExternalId,
            cancellationToken);

        if (denial is null)
            return Array.Empty<DenialActivityResponse>();

        var activities = await _denialRepository.GetActivitiesAsync(
            denial.Id,
            cancellationToken);

        return activities
            .Select(x => new DenialActivityResponse(
                x.ExternalId,
                x.ActivityType,
                x.Description,
                x.PerformedByUserId,
                x.ActivityDateUtc,
                x.CreatedAtUtc))
            .ToList();
    }

    public async Task<DenialActivityResponse?> AddActivityAsync(
        Guid denialExternalId,
        DenialActivityRequest request,
        CancellationToken cancellationToken = default)
    {
        var tenantId = RequireTenant();

        var denial = await _denialRepository.GetByExternalIdAsync(
            denialExternalId,
            cancellationToken);

        if (denial is null)
            return null;

        var activity = new DenialActivity(
            tenantId,
            denial.Id,
            request.ActivityType,
            request.Description);

        await _denialRepository.AddActivityAsync(
            activity,
            cancellationToken);

        await _denialRepository.SaveChangesAsync(cancellationToken);

        return new DenialActivityResponse(
            activity.ExternalId,
            activity.ActivityType,
            activity.Description,
            activity.PerformedByUserId,
            activity.ActivityDateUtc,
            activity.CreatedAtUtc);
    }


    public async Task<DenialResponse?> AppealAsync(
        Guid denialExternalId,
        AppealDenialRequest request,
        CancellationToken cancellationToken = default)
    {
        var tenantId = RequireTenant();

        var denial = await _denialRepository.GetByExternalIdAsync(
            denialExternalId,
            cancellationToken);

        if (denial is null)
            return null;

        // Change denial status from OPEN to APPEALED.
        denial.MarkAppealed();

        var description = string.IsNullOrWhiteSpace(request.AppealNotes)
            ? "Denial appeal submitted."
            : $"Denial appeal submitted. Notes: {request.AppealNotes.Trim()}";

        // Create an activity record for the appeal.
        var activity = new DenialActivity(
            tenantId,
            denial.Id,
            "APPEAL_SUBMITTED",
            description);

        _denialRepository.Update(denial);

        await _denialRepository.AddActivityAsync(
            activity,
            cancellationToken);

        // Save status change and activity together.
        await _denialRepository.SaveChangesAsync(cancellationToken);

        return await MapAsync(denial, cancellationToken);
    }

    public async Task<IReadOnlyList<DenialActivityResponse>> GetHistoryAsync(
        Guid denialExternalId,
        CancellationToken cancellationToken = default)
    {
        return await GetActivitiesAsync(
            denialExternalId,
            cancellationToken);
    }


    private long RequireTenant()
    {
        return _tenantContext.TenantId
            ?? throw new InvalidOperationException(
                "A tenant context is required for this operation.");
    }

    private async Task<DenialResponse> MapAsync(
        Denial denial,
        CancellationToken cancellationToken)
    {
        var claimExternalId =
            await _denialRepository.GetClaimExternalIdAsync(
                denial.ClaimId,
                cancellationToken);

        return new DenialResponse(
            denial.ExternalId,
            claimExternalId ?? Guid.Empty,
            denial.DenialNumber,
            denial.DenialCode,
            denial.DenialReason,
            denial.DenialCategory,
            denial.DeniedAmount,
            denial.DenialDate,
            denial.AppealDeadline,
            denial.Status,
            denial.AssignedToUserId,
            denial.CreatedAtUtc,
            denial.UpdatedAtUtc);
    }


    public async Task<DenialResponse?> AssignAsync(
        Guid externalId,
        long userId,
        CancellationToken cancellationToken = default)
    {
        RequireTenant();

        var denial = await _denialRepository.GetByExternalIdAsync(
            externalId,
            cancellationToken);

        if (denial is null)
            return null;

        denial.Assign(userId);

        _denialRepository.Update(denial);
        await _denialRepository.SaveChangesAsync(cancellationToken);

        return await MapAsync(denial, cancellationToken);
    }

    public async Task<DenialResponse?> UnassignAsync(
        Guid externalId,
        CancellationToken cancellationToken = default)
    {
        RequireTenant();

        var denial = await _denialRepository.GetByExternalIdAsync(
            externalId,
            cancellationToken);

        if (denial is null)
            return null;

        denial.Unassign();

        _denialRepository.Update(denial);
        await _denialRepository.SaveChangesAsync(cancellationToken);

        return await MapAsync(denial, cancellationToken);
    }
    // Denial Attachments

    public async Task<IReadOnlyList<DenialAttachmentResponse>> GetAttachmentsAsync(
        Guid denialExternalId,
        CancellationToken cancellationToken = default)
    {
        var denial = await _denialRepository.GetByExternalIdAsync(
            denialExternalId,
            cancellationToken);

        if (denial is null)
            return Array.Empty<DenialAttachmentResponse>();

        var attachments = await _denialRepository.GetAttachmentsAsync(
            denial.Id,
            cancellationToken);

        return attachments.Select(MapAttachment).ToList();
    }

    public async Task<DenialAttachmentResponse?> AddAttachmentAsync(
        Guid denialExternalId,
        CreateDenialAttachmentRequest request,
        CancellationToken cancellationToken = default)
    {
        var denial = await _denialRepository.GetByExternalIdAsync(
            denialExternalId,
            cancellationToken);

        if (denial is null)
            return null;

        if (string.IsNullOrWhiteSpace(request.FileName))
            throw new ArgumentException("File name is required.");

        if (string.IsNullOrWhiteSpace(request.ContentType))
            throw new ArgumentException("Content type is required.");

        if (request.FileSizeBytes <= 0)
            throw new ArgumentException("File size must be greater than zero.");

        if (string.IsNullOrWhiteSpace(request.StorageReference))
            throw new ArgumentException("Storage reference is required.");

        var attachment = new DenialAttachment(
            _tenantContext.TenantId!.Value,
            denial.Id,
            request.FileName,
            request.ContentType,
            request.FileSizeBytes,
            request.StorageReference);

        await _denialRepository.AddAttachmentAsync(
            attachment,
            cancellationToken);

        await _denialRepository.SaveChangesAsync(cancellationToken);

        return MapAttachment(attachment);
    }

    public async Task<bool> DeleteAttachmentAsync(
        Guid denialExternalId,
        Guid attachmentExternalId,
        CancellationToken cancellationToken = default)
    {
        var denial = await _denialRepository.GetByExternalIdAsync(
            denialExternalId,
            cancellationToken);

        if (denial is null)
            return false;

        var attachment = await _denialRepository.GetAttachmentByExternalIdAsync(
            denial.Id,
            attachmentExternalId,
            cancellationToken);

        if (attachment is null)
            return false;

        _denialRepository.RemoveAttachment(attachment);

        await _denialRepository.SaveChangesAsync(cancellationToken);

        return true;
    }

    private static DenialAttachmentResponse MapAttachment(
        DenialAttachment attachment)
    {
        return new DenialAttachmentResponse
        {
            ExternalId = attachment.ExternalId,
            FileName = attachment.FileName,
            ContentType = attachment.ContentType,
            FileSizeBytes = attachment.FileSizeBytes,
            StorageReference = attachment.StorageReference,
            UploadedByUserId = attachment.UploadedByUserId,
            UploadedAtUtc = attachment.UploadedAtUtc,
            CreatedAtUtc = attachment.CreatedAtUtc
        };
    }
    // Denial Payer Responses

    public async Task<IReadOnlyList<DenialPayerResponseResponse>> GetPayerResponsesAsync(
        Guid denialExternalId,
        CancellationToken cancellationToken = default)
    {
        var denial = await _denialRepository.GetByExternalIdAsync(
            denialExternalId,
            cancellationToken);

        if (denial is null)
            return Array.Empty<DenialPayerResponseResponse>();

        var responses = await _denialRepository.GetPayerResponsesAsync(
            denial.Id,
            cancellationToken);

        return responses.Select(MapPayerResponse).ToList();
    }

    public async Task<DenialPayerResponseResponse?> AddPayerResponseAsync(
        Guid denialExternalId,
        CreateDenialPayerResponseRequest request,
        CancellationToken cancellationToken = default)
    {
        var denial = await _denialRepository.GetByExternalIdAsync(
            denialExternalId,
            cancellationToken);

        if (denial is null)
            return null;

        if (string.IsNullOrWhiteSpace(request.ResponseType))
            throw new ArgumentException("Response type is required.");

        if (string.IsNullOrWhiteSpace(request.ResponseMessage))
            throw new ArgumentException("Response message is required.");

        if (request.ResponseDateUtc == default)
            throw new ArgumentException("Response date is required.");

        var response = new DenialPayerResponse
        {
            TenantId = _tenantContext.TenantId
                ?? throw new InvalidOperationException("Tenant context is required."),
            DenialId = denial.Id,
            ResponseType = request.ResponseType.Trim(),
            ResponseCode = request.ResponseCode?.Trim(),
            ResponseMessage = request.ResponseMessage.Trim(),
            PayerReferenceNumber = request.PayerReferenceNumber?.Trim(),
            ResponseDateUtc = DateTime.SpecifyKind(
                request.ResponseDateUtc,
                DateTimeKind.Utc)
        };

        await _denialRepository.AddPayerResponseAsync(
            response,
            cancellationToken);

        await _denialRepository.SaveChangesAsync(cancellationToken);

        return MapPayerResponse(response);
    }

    private static DenialPayerResponseResponse MapPayerResponse(
        DenialPayerResponse response)
    {
        return new DenialPayerResponseResponse
        {
            ExternalId = response.ExternalId,
            ResponseType = response.ResponseType,
            ResponseCode = response.ResponseCode,
            ResponseMessage = response.ResponseMessage,
            PayerReferenceNumber = response.PayerReferenceNumber,
            ResponseDateUtc = response.ResponseDateUtc,
            ReceivedByUserId = response.ReceivedByUserId,
            CreatedAtUtc = response.CreatedAtUtc
        };
    }

    // Denial Resolutions

    public async Task<IReadOnlyList<DenialResolutionResponse>> GetResolutionsAsync(
        Guid denialExternalId,
        CancellationToken cancellationToken = default)
    {
        var denial = await _denialRepository.GetByExternalIdAsync(
            denialExternalId,
            cancellationToken);

        if (denial is null)
            return Array.Empty<DenialResolutionResponse>();

        var resolutions = await _denialRepository.GetResolutionsAsync(
            denial.Id,
            cancellationToken);

        return resolutions.Select(MapResolution).ToList();
    }

    public async Task<DenialResolutionResponse?> AddResolutionAsync(
        Guid denialExternalId,
        CreateDenialResolutionRequest request,
        CancellationToken cancellationToken = default)
    {
        var denial = await _denialRepository.GetByExternalIdAsync(
            denialExternalId,
            cancellationToken);

        if (denial is null)
            return null;

        if (string.IsNullOrWhiteSpace(request.ResolutionType))
            throw new ArgumentException("Resolution type is required.");

        if (string.IsNullOrWhiteSpace(request.ResolutionNotes))
            throw new ArgumentException("Resolution notes are required.");

        if (request.ResolvedAmount < 0)
            throw new ArgumentException("Resolved amount cannot be negative.");

        if (request.ResolutionDateUtc == default)
            throw new ArgumentException("Resolution date is required.");

        var resolution = new DenialResolution
        {
            TenantId = _tenantContext.TenantId
                ?? throw new InvalidOperationException("Tenant context is required."),
            DenialId = denial.Id,
            ResolutionType = request.ResolutionType.Trim(),
            ResolutionNotes = request.ResolutionNotes.Trim(),
            ResolvedAmount = request.ResolvedAmount,
            ResolutionDateUtc = DateTime.SpecifyKind(
                request.ResolutionDateUtc,
                DateTimeKind.Utc)
        };

        await _denialRepository.AddResolutionAsync(
            resolution,
            cancellationToken);

        await _denialRepository.SaveChangesAsync(cancellationToken);

        return MapResolution(resolution);
    }

    private static DenialResolutionResponse MapResolution(
        DenialResolution resolution)
    {
        return new DenialResolutionResponse
        {
            ExternalId = resolution.ExternalId,
            ResolutionType = resolution.ResolutionType,
            ResolutionNotes = resolution.ResolutionNotes,
            ResolvedAmount = resolution.ResolvedAmount,
            ResolutionDateUtc = resolution.ResolutionDateUtc,
            ResolvedByUserId = resolution.ResolvedByUserId,
            CreatedAtUtc = resolution.CreatedAtUtc
        };
    }

    public async Task<IReadOnlyList<DenialReasonResponse>> GetReasonsAsync(
    bool? isActive,
    CancellationToken cancellationToken = default)
    {
        var reasons = await _denialRepository.GetReasonsAsync(
            isActive,
            cancellationToken);

        return reasons.Select(MapReason).ToList();
    }

    public async Task<DenialReasonResponse> CreateReasonAsync(
        CreateDenialReasonRequest request,
        CancellationToken cancellationToken = default)
    {
        RequireTenant();

        if (string.IsNullOrWhiteSpace(request.ReasonCode))
            throw new ArgumentException("Reason code is required.");

        if (string.IsNullOrWhiteSpace(request.ReasonName))
            throw new ArgumentException("Reason name is required.");

        if (string.IsNullOrWhiteSpace(request.Category))
            throw new ArgumentException("Reason category is required.");

        var reasonCode = request.ReasonCode.Trim();

        if (await _denialRepository.ExistsReasonCodeAsync(
            reasonCode,
            cancellationToken: cancellationToken))
        {
            throw new InvalidOperationException(
                "A denial reason with this code already exists.");
        }

        var reason = new DenialReason
        {
            TenantId = _tenantContext.TenantId!.Value,
            ReasonCode = reasonCode,
            ReasonName = request.ReasonName.Trim(),
            Description = request.Description?.Trim(),
            Category = request.Category.Trim(),
            IsActive = true
        };

        await _denialRepository.AddReasonAsync(
            reason,
            cancellationToken);

        await _denialRepository.SaveChangesAsync(cancellationToken);

        return MapReason(reason);
    }

    public async Task<DenialReasonResponse?> UpdateReasonAsync(
        Guid externalId,
        UpdateDenialReasonRequest request,
        CancellationToken cancellationToken = default)
    {
        RequireTenant();

        var reason = await _denialRepository.GetReasonByExternalIdAsync(
            externalId,
            cancellationToken);

        if (reason is null)
            return null;

        if (string.IsNullOrWhiteSpace(request.ReasonName))
            throw new ArgumentException("Reason name is required.");

        if (string.IsNullOrWhiteSpace(request.Category))
            throw new ArgumentException("Reason category is required.");

        reason.ReasonName = request.ReasonName.Trim();
        reason.Description = request.Description?.Trim();
        reason.Category = request.Category.Trim();
        reason.IsActive = request.IsActive;

        _denialRepository.UpdateReason(reason);

        await _denialRepository.SaveChangesAsync(cancellationToken);

        return MapReason(reason);
    }

    private static DenialReasonResponse MapReason(DenialReason reason)
    {
        return new DenialReasonResponse(
            reason.ExternalId,
            reason.ReasonCode,
            reason.ReasonName,
            reason.Description,
            reason.Category,
            reason.IsActive,
            reason.CreatedAtUtc,
            reason.UpdatedAtUtc);
    }
}
