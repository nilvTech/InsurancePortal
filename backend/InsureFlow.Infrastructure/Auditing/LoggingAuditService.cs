using InsureFlow.Application.Abstractions;
using Microsoft.Extensions.Logging;

namespace InsureFlow.Infrastructure.Auditing;

public sealed class LoggingAuditService(ILogger<LoggingAuditService> logger) : IAuditService
{
    public Task WriteAsync(AuditEntry entry, CancellationToken cancellationToken = default)
    {
        logger.LogInformation(
            "Audit event: Action={Action}, EntityName={EntityName}, EntityId={EntityId}, TenantId={TenantId}, UserId={UserId}, CorrelationId={CorrelationId}, Reason={Reason}",
            entry.Action,
            entry.EntityName,
            entry.EntityId,
            entry.TenantId,
            entry.UserId,
            entry.CorrelationId,
            entry.Reason);

        return Task.CompletedTask;
    }
}
