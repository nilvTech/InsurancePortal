
using InsureFlow.Application.Organizations;
using InsureFlow.Application.Providers;
using InsureFlow.Application.Tenants;
using InsureFlow.Application.Payers;
using InsureFlow.Application.Patients;
using InsureFlow.Application.InsurancePlans;
using InsureFlow.Application.PatientInsurances;
using InsureFlow.Application.Eligibility;
using InsureFlow.Application.Denials;
using InsureFlow.Application.Claims;

using Microsoft.Extensions.DependencyInjection;

namespace InsureFlow.Application.DependencyInjection;

public static class ApplicationServiceCollectionExtensions
{
    public static IServiceCollection AddInsureFlowApplication(
        this IServiceCollection services)
    {
        services.AddScoped<ITenantService, TenantService>();
        services.AddScoped<IOrganizationService, OrganizationService>();
        services.AddScoped<IProviderService, ProviderService>();
        services.AddScoped<IPayerService, PayerService>();
        services.AddScoped<IPatientService, PatientService>();
        services.AddScoped<IInsurancePlanService, InsurancePlanService>();
        services.AddScoped<IPatientInsuranceService, PatientInsuranceService>();
        services.AddScoped<IEligibilityService, EligibilityService>();
        services.AddScoped<IClaimService, ClaimService>();
        services.AddScoped<IDenialService, DenialService>();
        return services;
    }
}
