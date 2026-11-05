
using InsureFlow.Application.Tenants;
using InsureFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;

using EligibilityHistoryEntity =
    InsureFlow.Domain.Entities.EligibilityHistory;

using AuthorizationHistoryEntity =
    InsureFlow.Domain.Entities.AuthorizationHistory;

namespace InsureFlow.Infrastructure.Persistence;

public sealed class InsureFlowDbContext : DbContext
{
    private readonly ITenantContext _tenantContext;

    public InsureFlowDbContext(
        DbContextOptions<InsureFlowDbContext> options,
        ITenantContext tenantContext)
        : base(options)
    {
        _tenantContext = tenantContext;
    }

    public DbSet<Tenant> Tenants =>
        Set<Tenant>();

    public DbSet<Organization> Organizations =>
        Set<Organization>();

    public DbSet<Provider> Providers =>
        Set<Provider>();

    public DbSet<Payer> Payers =>
        Set<Payer>();

    public DbSet<Patient> Patients =>
        Set<Patient>();

    public DbSet<InsurancePlan> InsurancePlans =>
        Set<InsurancePlan>();

    public DbSet<PatientInsurance> PatientInsurances =>
        Set<PatientInsurance>();

    public DbSet<Claim> Claims =>
        Set<Claim>();

    public DbSet<ClaimLine> ClaimLines =>
        Set<ClaimLine>();

    public DbSet<ClaimHistory> ClaimHistory =>
        Set<ClaimHistory>();

    public DbSet<ClaimRejection> ClaimRejections =>
        Set<ClaimRejection>();
        
    public DbSet<ClaimDiagnosis> ClaimDiagnoses =>
    Set<ClaimDiagnosis>();

public DbSet<ClaimAttachment> ClaimAttachments =>
    Set<ClaimAttachment>();    
    public DbSet<EligibilityRequest> EligibilityRequests =>
        Set<EligibilityRequest>();

    public DbSet<EligibilityResponse> EligibilityResponses =>
        Set<EligibilityResponse>();

    public DbSet<EligibilityBenefit> EligibilityBenefits =>
        Set<EligibilityBenefit>();

    public DbSet<EligibilityHistory> EligibilityHistory =>
        Set<EligibilityHistory>();

    public DbSet<Authorization> Authorizations =>
        Set<Authorization>();

    public DbSet<AuthorizationServiceLine> AuthorizationServiceLines =>
        Set<AuthorizationServiceLine>();

    public DbSet<AuthorizationDiagnosis> AuthorizationDiagnoses =>
        Set<AuthorizationDiagnosis>();

    public DbSet<AuthorizationNote> AuthorizationNotes =>
        Set<AuthorizationNote>();

    public DbSet<AuthorizationHistory> AuthorizationHistory =>
        Set<AuthorizationHistory>();

    public DbSet<AuthorizationDocument> AuthorizationDocuments =>
        Set<AuthorizationDocument>();

    public DbSet<AuthorizationAppeal> AuthorizationAppeals =>
        Set<AuthorizationAppeal>();

        

    /// <summary>
    /// Gets the tenant ID associated with the current request.
    /// </summary>
    public long? CurrentTenantId =>
        _tenantContext.TenantId;

    protected override void OnModelCreating(
        ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(InsureFlowDbContext).Assembly);

        ConfigureTenantFilters(modelBuilder);
    }

    public override int SaveChanges()
    {
        EnforceTenantWriteRules();

        return base.SaveChanges();
    }

    public override int SaveChanges(
        bool acceptAllChangesOnSuccess)
    {
        EnforceTenantWriteRules();

        return base.SaveChanges(
            acceptAllChangesOnSuccess);
    }

    public override Task<int> SaveChangesAsync(
        CancellationToken cancellationToken = default)
    {
        EnforceTenantWriteRules();

        return base.SaveChangesAsync(
            cancellationToken);
    }

    public override Task<int> SaveChangesAsync(
        bool acceptAllChangesOnSuccess,
        CancellationToken cancellationToken = default)
    {
        EnforceTenantWriteRules();

        return base.SaveChangesAsync(
            acceptAllChangesOnSuccess,
            cancellationToken);
    }

    private void ConfigureTenantFilters(
        ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Organization>()
            .HasQueryFilter(x =>
                x.TenantId == CurrentTenantId);

        modelBuilder.Entity<Provider>()
            .HasQueryFilter(x =>
                x.TenantId == CurrentTenantId);

        modelBuilder.Entity<Payer>()
            .HasQueryFilter(x =>
                x.TenantId == CurrentTenantId);

        modelBuilder.Entity<Patient>()
            .HasQueryFilter(x =>
                x.TenantId == CurrentTenantId);

        modelBuilder.Entity<InsurancePlan>()
            .HasQueryFilter(x =>
                x.TenantId == CurrentTenantId);

        modelBuilder.Entity<PatientInsurance>()
            .HasQueryFilter(x =>
                x.TenantId == CurrentTenantId);

        modelBuilder.Entity<Claim>()
            .HasQueryFilter(x =>
                x.TenantId == CurrentTenantId);

        modelBuilder.Entity<ClaimLine>()
            .HasQueryFilter(x =>
                x.TenantId == CurrentTenantId);

        modelBuilder.Entity<ClaimHistory>()
            .HasQueryFilter(x =>
                x.TenantId == CurrentTenantId);

        modelBuilder.Entity<ClaimRejection>()
            .HasQueryFilter(x => 
                x.TenantId == CurrentTenantId);

        modelBuilder.Entity<EligibilityRequest>()
            .HasQueryFilter(x =>
                x.TenantId == CurrentTenantId);

        modelBuilder.Entity<EligibilityResponse>()
            .HasQueryFilter(x =>
                x.TenantId == CurrentTenantId);

        modelBuilder.Entity<EligibilityBenefit>()
            .HasQueryFilter(x =>
                x.TenantId == CurrentTenantId);

        modelBuilder.Entity<EligibilityHistoryEntity>()
            .HasQueryFilter(x =>
                x.TenantId == CurrentTenantId);

        modelBuilder.Entity<AuthorizationHistory>()
            .HasQueryFilter(x =>
                x.TenantId == CurrentTenantId);

        modelBuilder.Entity<AuthorizationDocument>()
            .HasQueryFilter(x =>
                x.TenantId == CurrentTenantId);

        modelBuilder.Entity<AuthorizationAppeal>()
            .HasQueryFilter(x =>
                x.TenantId == CurrentTenantId);

        modelBuilder.Entity<Authorization>()
            .HasQueryFilter(x =>
                x.TenantId == CurrentTenantId);

        modelBuilder.Entity<AuthorizationServiceLine>()
            .HasQueryFilter(x =>
                x.TenantId == CurrentTenantId);

        modelBuilder.Entity<AuthorizationDiagnosis>()
            .HasQueryFilter(x =>
                x.TenantId == CurrentTenantId);

        modelBuilder.Entity<AuthorizationNote>()
            .HasQueryFilter(x =>
                x.TenantId == CurrentTenantId);
    }

    private void EnforceTenantWriteRules()
    {
        var tenantId = CurrentTenantId;

        foreach (var entry in ChangeTracker.Entries())
        {
            // Tenant is the tenant registry/root entity.
            // It is intentionally not tenant-owned.
            if (entry.Entity is Tenant)
            {
                continue;
            }

            if (!IsTenantOwnedEntity(entry.Entity))
            {
                continue;
            }

            if (!tenantId.HasValue)
            {
                throw new InvalidOperationException(
                    "A tenant context is required before modifying " +
                    "tenant-owned data.");
            }

            var tenantProperty =
                entry.Property("TenantId");

            var currentTenantId =
                tenantProperty.CurrentValue is long currentId
                    ? currentId
                    : 0;

            switch (entry.State)
            {
                case EntityState.Added:

                    if (currentTenantId != tenantId.Value)
                    {
                        throw new InvalidOperationException(
                            "The entity TenantId does not match " +
                            "the current tenant.");
                    }

                    break;

                case EntityState.Modified:
                case EntityState.Deleted:

                    var originalTenantId =
                        tenantProperty.OriginalValue is long originalId
                            ? originalId
                            : 0;

                    if (originalTenantId != tenantId.Value ||
                        currentTenantId != tenantId.Value)
                    {
                        throw new InvalidOperationException(
                            "The entity does not belong to " +
                            "the current tenant.");
                    }

                    break;
            }
        }
    }

    private static bool IsTenantOwnedEntity(
        object entity)
    {
        return entity is Organization
            or Provider
            or Payer
            or Patient
            or InsurancePlan
            or PatientInsurance
            or EligibilityRequest
            or EligibilityResponse
            or EligibilityBenefit
            or EligibilityHistoryEntity
            or AuthorizationHistoryEntity
            or AuthorizationDocument
            or AuthorizationAppeal
            or Authorization
            or AuthorizationServiceLine
            or AuthorizationDiagnosis
            or AuthorizationNote
            or Claim
            or ClaimLine
            or ClaimDiagnosis
            or ClaimAttachment
            or InsureFlow.Domain.Entities.ClaimHistory
            or InsureFlow.Domain.Entities.ClaimRejection;
    }
}

