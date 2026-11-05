
using InsureFlow.Application.Authorizations;
using InsureFlow.Contracts.Authorizations;
using Microsoft.AspNetCore.Mvc;

namespace InsureFlow.Api.Controllers;

[ApiController]
[Route("api/v1/authorizations")]
public sealed class AuthorizationsController(
    IAuthorizationService service) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get(
        [FromQuery] AuthorizationListRequest request,
        CancellationToken ct)
        => Ok(await service.GetPagedAsync(request, ct));

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(
        Guid id,
        CancellationToken ct)
    {
        var result = await service.GetByIdAsync(id, ct);

        return result is null
            ? NotFound()
            : Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateAuthorizationRequest request,
        CancellationToken ct)
    {
        var result = await service.CreateAsync(request, ct);

        return CreatedAtAction(
            nameof(GetById),
            new { id = result.Id },
            result);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(
        Guid id,
        [FromBody] UpdateAuthorizationRequest request,
        CancellationToken ct)
        => await service.UpdateAsync(id, request, ct)
            ? NoContent()
            : NotFound();

    [HttpPost("{id:guid}/submit")]
    public async Task<IActionResult> Submit(
        Guid id,
        [FromBody] AuthorizationActionRequest? request,
        CancellationToken ct)
        => await service.SubmitAsync(id, request?.Note, ct)
            ? NoContent()
            : NotFound();

    [HttpPost("{id:guid}/approve")]
    public async Task<IActionResult> Approve(
        Guid id,
        [FromBody] AuthorizationActionRequest? request,
        CancellationToken ct)
        => await service.ApproveAsync(id, request?.Note, ct)
            ? NoContent()
            : NotFound();

    [HttpPost("{id:guid}/deny")]
    public async Task<IActionResult> Deny(
        Guid id,
        [FromBody] DenyAuthorizationRequest request,
        CancellationToken ct)
        => await service.DenyAsync(id, request.Reason, ct)
            ? NoContent()
            : NotFound();

    [HttpPost("{id:guid}/cancel")]
    public async Task<IActionResult> Cancel(
        Guid id,
        [FromBody] AuthorizationActionRequest? request,
        CancellationToken ct)
        => await service.CancelAsync(id, request?.Note, ct)
            ? NoContent()
            : NotFound();

    [HttpGet("{id:guid}/services")]
    public async Task<IActionResult> GetServices(
        Guid id,
        CancellationToken ct)
        => Ok(await service.GetServicesAsync(id, ct));

    [HttpPost("{id:guid}/services")]
    public async Task<IActionResult> AddService(
        Guid id,
        [FromBody] AddAuthorizationServiceLineRequest request,
        CancellationToken ct)
        => Ok(await service.AddServiceAsync(id, request, ct));

    [HttpPut("{id:guid}/services/{serviceId:long}")]
    public async Task<IActionResult> UpdateService(
        Guid id,
        long serviceId,
        [FromBody] UpdateAuthorizationServiceLineRequest request,
        CancellationToken ct)
        => await service.UpdateServiceAsync(
            id, serviceId, request, ct)
            ? NoContent()
            : NotFound();

    [HttpDelete("{id:guid}/services/{serviceId:long}")]
    public async Task<IActionResult> DeleteService(
        Guid id,
        long serviceId,
        CancellationToken ct)
        => await service.DeleteServiceAsync(
            id, serviceId, ct)
            ? NoContent()
            : NotFound();

    [HttpGet("{id:guid}/diagnoses")]
    public async Task<IActionResult> GetDiagnoses(
        Guid id,
        CancellationToken ct)
        => Ok(await service.GetDiagnosesAsync(id, ct));

    [HttpPost("{id:guid}/diagnoses")]
    public async Task<IActionResult> AddDiagnosis(
        Guid id,
        [FromBody] AddAuthorizationDiagnosisRequest request,
        CancellationToken ct)
        => Ok(await service.AddDiagnosisAsync(id, request, ct));

    [HttpDelete("{id:guid}/diagnoses/{diagnosisId:long}")]
    public async Task<IActionResult> DeleteDiagnosis(
        Guid id,
        long diagnosisId,
        CancellationToken ct)
        => await service.DeleteDiagnosisAsync(
            id, diagnosisId, ct)
            ? NoContent()
            : NotFound();

    [HttpGet("{id:guid}/documents")]
    public async Task<IActionResult> GetDocuments(
        Guid id,
        CancellationToken ct)
        => Ok(await service.GetDocumentsAsync(id, ct));

    [HttpPost("{id:guid}/documents")]
    public async Task<IActionResult> AddDocument(
        Guid id,
        [FromBody] AddAuthorizationDocumentRequest request,
        CancellationToken ct)
        => Ok(await service.AddDocumentAsync(id, request, ct));

    [HttpDelete("{id:guid}/documents/{documentId:long}")]
    public async Task<IActionResult> DeleteDocument(
        Guid id,
        long documentId,
        CancellationToken ct)
        => await service.DeleteDocumentAsync(
            id, documentId, ct)
            ? NoContent()
            : NotFound();

    [HttpGet("{id:guid}/history")]
    public async Task<IActionResult> GetHistory(
        Guid id,
        CancellationToken ct)
        => Ok(await service.GetHistoryAsync(id, ct));

    [HttpPost("{id:guid}/appeal")]
    public async Task<IActionResult> Appeal(
        Guid id,
        [FromBody] CreateAuthorizationAppealRequest request,
        CancellationToken ct)
        => Ok(await service.CreateAppealAsync(id, request, ct));

    // Retained compatibility endpoints from the earlier overlay.

    [HttpGet("{id:guid}/notes")]
    public async Task<IActionResult> GetNotes(
        Guid id,
        CancellationToken ct)
        => Ok(await service.GetNotesAsync(id, ct));

    [HttpPost("{id:guid}/notes")]
    public async Task<IActionResult> AddNote(
        Guid id,
        [FromBody] AddAuthorizationNoteRequest request,
        CancellationToken ct)
        => Ok(await service.AddNoteAsync(id, request, ct));
}