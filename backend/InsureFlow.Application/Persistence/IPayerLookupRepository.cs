using InsureFlow.Domain.Entities;

namespace InsureFlow.Application.Persistence;

/// <summary>
/// Provides tenant-scoped payer lookups for authorization workflows.
/// </summary>
public interface IPayerLookupRepository
{
    Task<Payer?> GetByExternalIdAsync(
        Guid externalId,
        CancellationToken cancellationToken = default);
}
