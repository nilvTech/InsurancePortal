namespace InsureFlow.Application.Tenants;

public interface ITenantContext
{
    long? TenantId { get; }

    bool HasTenant { get; }
}
