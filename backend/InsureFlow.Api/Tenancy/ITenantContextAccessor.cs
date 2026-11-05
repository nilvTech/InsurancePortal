namespace InsureFlow.Api.Tenancy;

public interface ITenantContextAccessor
{
    void SetTenant(long tenantId);

    void Clear();
}
