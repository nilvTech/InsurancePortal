using InsureFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InsureFlow.Infrastructure.Persistence.Configurations;

public sealed class EligibilityRequestConfiguration
    : IEntityTypeConfiguration<EligibilityRequest>
{
    public void Configure(EntityTypeBuilder<EligibilityRequest> builder)
    {
        builder.ToTable("eligibility_requests");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnName("id")
            .ValueGeneratedOnAdd();

        builder.Property(x => x.ExternalId)
            .HasColumnName("request_guid")
            .HasMaxLength(36)
            .IsRequired();

        builder.HasIndex(x => x.ExternalId)
            .IsUnique()
            .HasDatabaseName("uk_eligibility_requests_request_guid");

        builder.Property(x => x.TenantId)
            .HasColumnName("tenant_id")
            .IsRequired();

        builder.HasIndex(x => x.TenantId)
            .HasDatabaseName("idx_eligibility_requests_tenant_id");

        builder.Property(x => x.PatientId)
            .HasColumnName("patient_id")
            .IsRequired();

        builder.HasIndex(x => x.PatientId)
            .HasDatabaseName("idx_eligibility_requests_patient_id");

        builder.Property(x => x.PatientInsuranceId)
            .HasColumnName("patient_insurance_id")
            .IsRequired();

        builder.HasIndex(x => x.PatientInsuranceId)
            .HasDatabaseName("idx_eligibility_requests_patient_insurance_id");

        builder.Property(x => x.PayerId)
            .HasColumnName("payer_id")
            .IsRequired();

        builder.HasIndex(x => x.PayerId)
            .HasDatabaseName("idx_eligibility_requests_payer_id");

        builder.Property(x => x.RequestNumber)
            .HasColumnName("request_number")
            .HasMaxLength(50)
            .IsRequired();

        builder.HasIndex(x => x.RequestNumber)
            .IsUnique()
            .HasDatabaseName("uk_eligibility_requests_request_number");

        builder.Property(x => x.RequestType)
            .HasColumnName("request_type")
            .HasConversion<string>()
            .HasMaxLength(30)
            .IsRequired();

        builder.Property(x => x.Status)
            .HasColumnName("status")
            .HasConversion<string>()
            .HasMaxLength(30)
            .IsRequired();

        builder.HasIndex(x => x.Status)
            .HasDatabaseName("idx_eligibility_requests_status");

        builder.Property(x => x.Source)
            .HasColumnName("source")
            .HasConversion<string>()
            .HasMaxLength(30)
            .IsRequired();

        builder.Property(x => x.RequestedAt)
            .HasColumnName("requested_at")
            .HasColumnType("datetime(6)")
            .IsRequired();

        builder.HasIndex(x => x.RequestedAt)
            .HasDatabaseName("idx_eligibility_requests_requested_at");

        builder.Property(x => x.CompletedAt)
            .HasColumnName("completed_at")
            .HasColumnType("datetime(6)");

        builder.Property(x => x.ErrorCode)
            .HasColumnName("error_code")
            .HasMaxLength(50);

        builder.Property(x => x.ErrorMessage)
            .HasColumnName("error_message")
            .HasMaxLength(1000);

        builder.Property(x => x.TraceId)
            .HasColumnName("trace_id")
            .HasMaxLength(100);

        builder.Property(x => x.RawRequest)
            .HasColumnName("raw_request")
            .HasColumnType("json");

        builder.Property(x => x.CreatedAtUtc)
            .HasColumnName("created_at")
            .HasColumnType("datetime(6)")
            .IsRequired();

        builder.Property(x => x.UpdatedAtUtc)
            .HasColumnName("updated_at")
            .HasColumnType("datetime(6)");

        builder.Ignore(x => x.CreatedByUserId);
        builder.Ignore(x => x.UpdatedByUserId);

        builder.HasOne(x => x.Tenant)
            .WithMany()
            .HasForeignKey(x => x.TenantId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Patient)
            .WithMany()
            .HasForeignKey(x => x.PatientId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.PatientInsurance)
            .WithMany()
            .HasForeignKey(x => x.PatientInsuranceId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Payer)
            .WithMany()
            .HasForeignKey(x => x.PayerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Response)
            .WithOne(x => x.EligibilityRequest)
            .HasForeignKey<EligibilityResponse>(
                x => x.EligibilityRequestId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.History)
            .WithOne(x => x.EligibilityRequest)
            .HasForeignKey(x => x.EligibilityRequestId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}