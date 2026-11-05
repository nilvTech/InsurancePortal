namespace InsureFlow.Application.Abstractions;

public interface IAuditService
{
    Task WriteAsync(AuditEntry entry, CancellationToken cancellationToken = default);
}

public sealed record AuditEntry(
    long? TenantId,
    long? UserId,
    string Action,
    string EntityName,
    string? EntityId = null,
    string? CorrelationId = null,
    string? Reason = null);
