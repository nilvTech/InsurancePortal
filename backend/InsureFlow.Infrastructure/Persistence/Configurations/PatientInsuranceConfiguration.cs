using InsureFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InsureFlow.Infrastructure.Persistence.Configurations;

public sealed class PatientInsuranceConfiguration
    : IEntityTypeConfiguration<PatientInsurance>
{
    public void Configure(
        EntityTypeBuilder<PatientInsurance> builder)
    {
        builder.ToTable("patient_insurances");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnName("id")
            .ValueGeneratedOnAdd();

        builder.Property(x => x.ExternalId)
            .HasColumnName("patient_insurance_uuid")
            .HasMaxLength(36)
            .IsRequired();

        builder.HasIndex(x => x.ExternalId)
            .IsUnique()
            .HasDatabaseName(
                "uk_patient_insurances_patient_insurance_uuid");

        builder.Property(x => x.TenantId)
            .HasColumnName("tenant_id")
            .IsRequired();

        builder.HasIndex(x => x.TenantId)
            .HasDatabaseName(
                "idx_patient_insurances_tenant_id");

        builder.Property(x => x.PatientId)
            .HasColumnName("patient_id")
            .IsRequired();

        builder.HasIndex(x => x.PatientId)
            .HasDatabaseName(
                "idx_patient_insurances_patient_id");

        builder.Property(x => x.PayerId)
            .HasColumnName("payer_id")
            .IsRequired();

        builder.HasIndex(x => x.PayerId)
            .HasDatabaseName(
                "idx_patient_insurances_payer_id");

        builder.Property(x => x.InsurancePlanId)
            .HasColumnName("insurance_plan_id");

        builder.HasIndex(x => x.InsurancePlanId)
            .HasDatabaseName(
                "idx_patient_insurances_insurance_plan_id");

        builder.Property(x => x.InsuranceType)
            .HasColumnName("insurance_type")
            .HasConversion<string>()
            .HasMaxLength(30)
            .IsRequired();

        builder.Property(x => x.CoveragePriority)
            .HasColumnName("coverage_priority")
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.HasIndex(x => x.CoveragePriority)
            .HasDatabaseName(
                "idx_patient_insurances_coverage_priority");

        builder.Property(x => x.InsuranceStatus)
            .HasColumnName("insurance_status")
            .HasConversion<string>()
            .HasMaxLength(30)
            .IsRequired();

        builder.HasIndex(x => x.InsuranceStatus)
            .HasDatabaseName(
                "idx_patient_insurances_insurance_status");

        builder.Property(x => x.MemberId)
            .HasColumnName("member_id")
            .HasMaxLength(100)
            .IsRequired();

        builder.HasIndex(x => x.MemberId)
            .HasDatabaseName(
                "idx_patient_insurances_member_id");

        builder.Property(x => x.GroupNumber)
            .HasColumnName("group_number")
            .HasMaxLength(100);

        builder.Property(x => x.SubscriberId)
            .HasColumnName("subscriber_id")
            .HasMaxLength(100);

        builder.Property(x => x.SubscriberFirstName)
            .HasColumnName("subscriber_first_name")
            .HasMaxLength(100);

        builder.Property(x => x.SubscriberLastName)
            .HasColumnName("subscriber_last_name")
            .HasMaxLength(100);

        builder.Property(x => x.SubscriberRelationship)
            .HasColumnName("subscriber_relationship")
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(x => x.EffectiveDate)
            .HasColumnName("effective_date")
            .HasColumnType("date");

        builder.Property(x => x.TerminationDate)
            .HasColumnName("termination_date")
            .HasColumnType("date");

        builder.Property(x => x.PayerIdDisplay)
            .HasColumnName("payer_id_display")
            .HasMaxLength(50);

        builder.Property(x => x.EligibilityStatus)
            .HasColumnName("eligibility_status")
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.HasIndex(x => x.EligibilityStatus)
            .HasDatabaseName(
                "idx_patient_insurances_eligibility_status");

        builder.Property(x => x.LastVerifiedAt)
            .HasColumnName("last_verified_at")
            .HasColumnType("datetime(6)");

        builder.Property(x => x.IsActive)
            .HasColumnName("is_active")
            .IsRequired();

        builder.Property(x => x.CreatedAtUtc)
            .HasColumnName("created_at")
            .HasColumnType("datetime(6)")
            .IsRequired();

        builder.Property(x => x.UpdatedAtUtc)
            .HasColumnName("updated_at")
            .HasColumnType("datetime(6)");

        // The existing patient_insurances table does not contain
        // created_by_user_id or updated_by_user_id columns.
        builder.Ignore(x => x.CreatedByUserId);
        builder.Ignore(x => x.UpdatedByUserId);

        builder.HasOne(x => x.Tenant)
            .WithMany()
            .HasForeignKey(x => x.TenantId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Patient)
            .WithMany()
            .HasForeignKey(x => x.PatientId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Payer)
            .WithMany()
            .HasForeignKey(x => x.PayerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.InsurancePlan)
            .WithMany()
            .HasForeignKey(x => x.InsurancePlanId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}