using InsureFlow.Domain.Entities;
using InsureFlow.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InsureFlow.Infrastructure.Persistence.Configurations;

public sealed class ProviderConfiguration
    : IEntityTypeConfiguration<Provider>
{
    public void Configure(EntityTypeBuilder<Provider> builder)
    {
        builder.ToTable("providers");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnName("id")
            .HasColumnType("bigint unsigned")
            .ValueGeneratedOnAdd();

        builder.Property(x => x.ExternalId)
            .HasColumnName("provider_uuid")
            .HasColumnType("char(36)")
            .IsRequired();

        builder.HasIndex(x => x.ExternalId)
            .IsUnique()
            .HasDatabaseName("uk_providers_provider_uuid");

        builder.Property(x => x.TenantId)
            .HasColumnName("tenant_id")
            .HasColumnType("bigint unsigned")
            .IsRequired();

        builder.Property(x => x.OrganizationId)
            .HasColumnName("organization_id")
            .HasColumnType("bigint unsigned")
            .IsRequired();

        builder.HasIndex(x => x.TenantId)
            .HasDatabaseName("idx_providers_tenant_id");

        builder.HasIndex(x => x.OrganizationId)
            .HasDatabaseName("idx_providers_organization_id");

        builder.HasIndex(x => new { x.TenantId, x.Code })
            .IsUnique()
            .HasDatabaseName("uk_providers_1");

        builder.HasIndex(x => new { x.TenantId, x.Npi })
            .IsUnique()
            .HasDatabaseName("uk_providers_2");

        builder.Property(x => x.Code)
            .HasColumnName("provider_code")
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.FirstName)
            .HasColumnName("first_name")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.MiddleName)
            .HasColumnName("middle_name")
            .HasMaxLength(100);

        builder.Property(x => x.LastName)
            .HasColumnName("last_name")
            .HasMaxLength(100)
            .IsRequired();

        builder.HasIndex(x => x.LastName)
            .HasDatabaseName("idx_providers_last_name");

        builder.Property(x => x.Credential)
            .HasColumnName("credential")
            .HasMaxLength(50);

        builder.Property(x => x.Npi)
            .HasColumnName("npi")
            .HasMaxLength(10)
            .IsRequired();

        builder.Property(x => x.Specialty)
            .HasColumnName("specialty")
            .HasMaxLength(150);

        builder.HasIndex(x => x.Specialty)
            .HasDatabaseName("idx_providers_specialty");

        builder.Property(x => x.Department)
            .HasColumnName("department")
            .HasMaxLength(150);

        builder.Property(x => x.Phone)
            .HasColumnName("phone")
            .HasMaxLength(30);

        builder.Property(x => x.Email)
            .HasColumnName("email")
            .HasMaxLength(254);

        builder.Property(x => x.Status)
            .HasColumnName("status")
            .HasColumnType("enum('ACTIVE','INACTIVE')")
            .HasConversion(
                value => value == RecordStatus.Active
                    ? "ACTIVE"
                    : "INACTIVE",
                value => value == "ACTIVE"
                    ? RecordStatus.Active
                    : RecordStatus.Inactive)
            .IsRequired();

        builder.HasIndex(x => x.Status)
            .HasDatabaseName("idx_providers_status");

        builder.Property(x => x.CreatedAtUtc)
            .HasColumnName("created_at")
            .HasColumnType("datetime(6)")
            .IsRequired();

        builder.Property(x => x.UpdatedAtUtc)
            .HasColumnName("updated_at")
            .HasColumnType("datetime(6)")
            .IsRequired();

        builder.Ignore(x => x.CreatedByUserId);
        builder.Ignore(x => x.UpdatedByUserId);
        
        builder.HasOne(x => x.Tenant)
            .WithMany()
            .HasForeignKey(x => x.TenantId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("fk_providers_tenant_id_1");

        builder.HasOne(x => x.Organization)
            .WithMany(x => x.Providers)
            .HasForeignKey(x => x.OrganizationId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("fk_providers_organization_id_2");
    }
}
