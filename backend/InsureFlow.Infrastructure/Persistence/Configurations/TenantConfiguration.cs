using InsureFlow.Domain.Entities;
using InsureFlow.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InsureFlow.Infrastructure.Persistence.Configurations;

public sealed class TenantConfiguration
    : IEntityTypeConfiguration<Tenant>
{
    public void Configure(EntityTypeBuilder<Tenant> builder)
    {
        builder.ToTable("tenants");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnName("id")
            .HasColumnType("bigint unsigned")
            .ValueGeneratedOnAdd();

        builder.Property(x => x.ExternalId)
            .HasColumnName("tenant_uuid")
            .HasColumnType("char(36)")
            .IsRequired();

        builder.HasIndex(x => x.ExternalId)
            .IsUnique()
            .HasDatabaseName("uk_tenants_tenant_uuid");

        builder.Property(x => x.Name)
            .HasColumnName("tenant_name")
            .HasMaxLength(200)
            .IsRequired();

        builder.HasIndex(x => x.Name)
            .HasDatabaseName("idx_tenants_tenant_name");

        builder.Property(x => x.Code)
            .HasColumnName("tenant_code")
            .HasMaxLength(50)
            .IsRequired();

        builder.HasIndex(x => x.Code)
            .IsUnique()
            .HasDatabaseName("uk_tenants_tenant_code");

        builder.Property(x => x.LegalName)
            .HasColumnName("legal_name")
            .HasMaxLength(250);

        builder.Property(x => x.TaxIdentifierLastFour)
            .HasColumnName("tax_identifier")
            .HasMaxLength(50);

        builder.HasIndex(x => x.TaxIdentifierLastFour)
            .IsUnique()
            .HasDatabaseName("uk_tenants_tax_identifier");

        builder.Property(x => x.Status)
            .HasColumnName("status")
            .HasColumnType(
                "enum('TRIAL','ACTIVE','SUSPENDED','INACTIVE')")
            .HasConversion(
                status =>
                    status == TenantStatus.Trial
                        ? "TRIAL"
                        : status == TenantStatus.Active
                            ? "ACTIVE"
                            : status == TenantStatus.Suspended
                                ? "SUSPENDED"
                                : "INACTIVE",
                value =>
                    value == "TRIAL"
                        ? TenantStatus.Trial
                        : value == "ACTIVE"
                            ? TenantStatus.Active
                            : value == "SUSPENDED"
                                ? TenantStatus.Suspended
                                : TenantStatus.Inactive)
            .IsRequired();

        builder.HasIndex(x => x.Status)
            .HasDatabaseName("idx_tenants_status");

        builder.Property(x => x.SubscriptionPlan)
            .HasColumnName("subscription_plan")
            .HasColumnType(
                "enum('STARTER','PROFESSIONAL','ENTERPRISE')")
            .HasConversion(
                value =>
                    value == SubscriptionPlan.Starter
                        ? "STARTER"
                        : value == SubscriptionPlan.Professional
                            ? "PROFESSIONAL"
                            : "ENTERPRISE",
                value =>
                    value == "STARTER"
                        ? SubscriptionPlan.Starter
                        : value == "PROFESSIONAL"
                            ? SubscriptionPlan.Professional
                            : SubscriptionPlan.Enterprise)
            .IsRequired();

        builder.Property(x => x.TimeZone)
            .HasColumnName("timezone")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.ContactFirstName)
            .HasColumnName("contact_first_name")
            .HasMaxLength(100);

        builder.Property(x => x.ContactLastName)
            .HasColumnName("contact_last_name")
            .HasMaxLength(100);

        builder.Property(x => x.ContactEmail)
            .HasColumnName("contact_email")
            .HasMaxLength(254);

        builder.Property(x => x.ContactPhone)
            .HasColumnName("contact_phone")
            .HasMaxLength(30);

        builder.Property(x => x.CreatedAtUtc)
            .HasColumnName("created_at")
            .HasColumnType("datetime(6)")
            .IsRequired();

        builder.Property(x => x.UpdatedAtUtc)
            .HasColumnName("updated_at")
            .HasColumnType("datetime(6)")
            .IsRequired();

        // These properties intentionally do not exist in the
        // current tenants table.
        builder.Ignore(x => x.CreatedByUserId);
        builder.Ignore(x => x.UpdatedByUserId);
        builder.Ignore(x => x.DefaultCurrency);
    }
}