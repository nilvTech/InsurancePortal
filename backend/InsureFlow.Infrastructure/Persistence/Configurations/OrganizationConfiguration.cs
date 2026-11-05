using InsureFlow.Domain.Entities;
using InsureFlow.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InsureFlow.Infrastructure.Persistence.Configurations;

public sealed class OrganizationConfiguration
    : IEntityTypeConfiguration<Organization>
{
    public void Configure(EntityTypeBuilder<Organization> builder)
    {
        builder.ToTable("organizations");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnName("id")
            .HasColumnType("bigint unsigned")
            .ValueGeneratedOnAdd();

        builder.Property(x => x.ExternalId)
            .HasColumnName("organization_uuid")
            .HasColumnType("char(36)")
            .IsRequired();

        builder.HasIndex(x => x.ExternalId)
            .IsUnique()
            .HasDatabaseName("uk_organizations_organization_uuid");

        builder.Property(x => x.TenantId)
            .HasColumnName("tenant_id")
            .HasColumnType("bigint unsigned")
            .IsRequired();

        builder.HasIndex(x => x.TenantId)
            .HasDatabaseName("idx_organizations_tenant_id");

        builder.Property(x => x.Code)
            .HasColumnName("organization_code")
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.Name)
            .HasColumnName("organization_name")
            .HasMaxLength(200)
            .IsRequired();

        builder.HasIndex(x => x.Name)
            .HasDatabaseName("idx_organizations_organization_name");

        builder.HasIndex(x => new { x.TenantId, x.Code })
            .IsUnique()
            .HasDatabaseName("uk_organizations_1");

        builder.Property(x => x.LegalName)
            .HasColumnName("legal_name")
            .HasMaxLength(250);

        builder.Property(x => x.Type)
            .HasColumnName("organization_type")
            .HasColumnType(
                "enum('MEDICAL_GROUP','HOSPITAL','HEALTH_SYSTEM','PRACTICE','OTHER')")
            .HasConversion(
                value => value == OrganizationType.MedicalGroup
                    ? "MEDICAL_GROUP"
                    : value == OrganizationType.Hospital
                        ? "HOSPITAL"
                        : value == OrganizationType.HealthSystem
                            ? "HEALTH_SYSTEM"
                            : value == OrganizationType.Practice
                                ? "PRACTICE"
                                : "OTHER",
                value => value == "MEDICAL_GROUP"
                    ? OrganizationType.MedicalGroup
                    : value == "HOSPITAL"
                        ? OrganizationType.Hospital
                        : value == "HEALTH_SYSTEM"
                            ? OrganizationType.HealthSystem
                            : value == "PRACTICE"
                                ? OrganizationType.Practice
                                : OrganizationType.Other)
            .IsRequired();

        builder.Property(x => x.TaxId)
            .HasColumnName("tax_id")
            .HasMaxLength(50);

        builder.Property(x => x.Npi)
            .HasColumnName("npi")
            .HasMaxLength(10);

        builder.HasIndex(x => new { x.TenantId, x.Npi })
            .IsUnique()
            .HasDatabaseName("uk_organizations_2");

        builder.Property(x => x.Phone)
            .HasColumnName("phone")
            .HasMaxLength(30);

        builder.Property(x => x.Fax)
            .HasColumnName("fax")
            .HasMaxLength(30);

        builder.Property(x => x.Email)
            .HasColumnName("email")
            .HasMaxLength(254);

        builder.Property(x => x.Website)
            .HasColumnName("website")
            .HasMaxLength(500);

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
            .HasDatabaseName("idx_organizations_status");

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
            .HasConstraintName("fk_organizations_tenant_id_1");
    }
}
