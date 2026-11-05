
using InsureFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InsureFlow.Infrastructure.Persistence.Configurations;

public sealed class ClaimConfiguration : IEntityTypeConfiguration<Claim>
{
    public void Configure(EntityTypeBuilder<Claim> builder)
    {
        builder.ToTable("claims");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnName("id")
            .ValueGeneratedOnAdd();

        builder.Property(x => x.ExternalId)
            .HasColumnName("claim_guid")
            .HasColumnType("char(36)")
            .HasMaxLength(36)
            .IsRequired();

        builder.HasIndex(x => x.ExternalId)
            .IsUnique()
            .HasDatabaseName("uk_claims_claim_guid");

        builder.Property(x => x.TenantId)
            .HasColumnName("tenant_id")
            .IsRequired();

        builder.HasIndex(x => x.TenantId)
            .HasDatabaseName("idx_claims_tenant_id");

        builder.Property(x => x.PatientId)
            .HasColumnName("patient_id")
            .IsRequired();

        builder.Property(x => x.PatientInsuranceId)
            .HasColumnName("patient_insurance_id")
            .IsRequired();

        builder.Property(x => x.PayerId)
            .HasColumnName("payer_id")
            .IsRequired();

        builder.Property(x => x.ProviderId)
            .HasColumnName("billing_provider_id")
            .IsRequired();

        builder.Property(x => x.ClaimNumber)
            .HasColumnName("claim_number")
            .HasMaxLength(60)
            .IsRequired();

        builder.HasIndex(x => new { x.TenantId, x.ClaimNumber })
            .IsUnique()
            .HasDatabaseName("uk_claims_1");

        builder.Property(x => x.ClaimType)
            .HasColumnName("claim_type")
            .HasMaxLength(30)
            .HasDefaultValue("PROFESSIONAL")
            .IsRequired();

        builder.Property(x => x.Status)
            .HasColumnName("status")
            .HasMaxLength(30)
            .HasDefaultValue("DRAFT")
            .IsRequired();

        builder.HasIndex(x => x.Status)
            .HasDatabaseName("idx_claims_status");

        builder.Property(x => x.ServiceFrom)
            .HasColumnName("service_from")
            .HasColumnType("date")
            .IsRequired();

        builder.Property(x => x.ServiceTo)
            .HasColumnName("service_to")
            .HasColumnType("date")
            .IsRequired();

        builder.Property(x => x.TotalChargeAmount)
            .HasColumnName("total_charge")
            .HasColumnType("decimal(12,2)")
            .HasDefaultValue(0m)
            .IsRequired();

        builder.Property(x => x.PayerClaimNumber)
            .HasColumnName("payer_claim_number")
            .HasMaxLength(100);

        builder.Property(x => x.SubmissionReference)
            .HasColumnName("submission_reference")
            .HasMaxLength(100);

        builder.Property(x => x.SubmittedAtUtc)
            .HasColumnName("submitted_at")
            .HasColumnType("datetime(6)");

        builder.Property(x => x.LastStatusCheckedAtUtc)
            .HasColumnName("last_status_checked_at")
            .HasColumnType("datetime(6)");

        builder.Property(x => x.Notes)
            .HasColumnName("notes")
            .HasColumnType("text");

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

        builder.HasOne<Tenant>()
            .WithMany()
            .HasForeignKey(x => x.TenantId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Patient>()
            .WithMany()
            .HasForeignKey(x => x.PatientId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<PatientInsurance>()
            .WithMany()
            .HasForeignKey(x => x.PatientInsuranceId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Payer>()
            .WithMany()
            .HasForeignKey(x => x.PayerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Provider>()
            .WithMany()
            .HasForeignKey(x => x.ProviderId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
