using InsureFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InsureFlow.Infrastructure.Persistence.Configurations;

public sealed class EligibilityResponseConfiguration
    : IEntityTypeConfiguration<EligibilityResponse>
{
    public void Configure(EntityTypeBuilder<EligibilityResponse> builder)
    {
        builder.ToTable("eligibility_responses");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnName("id")
            .ValueGeneratedOnAdd();

        builder.Property(x => x.ExternalId)
            .HasColumnName("response_guid")
            .HasMaxLength(36)
            .IsRequired();

        builder.HasIndex(x => x.ExternalId)
            .IsUnique()
            .HasDatabaseName("uk_eligibility_responses_response_guid");

        builder.Property(x => x.TenantId)
            .HasColumnName("tenant_id")
            .IsRequired();

        builder.HasIndex(x => x.TenantId)
            .HasDatabaseName("idx_eligibility_responses_tenant_id");

        builder.Property(x => x.EligibilityRequestId)
            .HasColumnName("eligibility_request_id")
            .IsRequired();

        builder.HasIndex(x => x.EligibilityRequestId)
            .IsUnique()
            .HasDatabaseName("uk_eligibility_responses_eligibility_request_id");

        builder.Property(x => x.Status)
            .HasColumnName("status")
            .HasConversion<string>()
            .HasMaxLength(30)
            .IsRequired();

        builder.HasIndex(x => x.Status)
            .HasDatabaseName("idx_eligibility_responses_status");

        builder.Property(x => x.PayerReference)
            .HasColumnName("payer_reference")
            .HasMaxLength(100);

        builder.Property(x => x.CoverageStatus)
            .HasColumnName("coverage_status")
            .HasConversion<string>()
            .HasMaxLength(30);

        builder.Property(x => x.EffectiveDate)
            .HasColumnName("effective_date")
            .HasColumnType("date");

        builder.Property(x => x.TerminationDate)
            .HasColumnName("termination_date")
            .HasColumnType("date");

        builder.Property(x => x.DeductibleIndividual)
            .HasColumnName("deductible_individual")
            .HasPrecision(18, 2);

        builder.Property(x => x.DeductibleRemaining)
            .HasColumnName("deductible_remaining")
            .HasPrecision(18, 2);

        builder.Property(x => x.Copay)
            .HasColumnName("copay")
            .HasPrecision(18, 2);

        builder.Property(x => x.CoinsurancePercent)
            .HasColumnName("coinsurance_percent")
            .HasPrecision(5, 2);

        builder.Property(x => x.ResponseMessage)
            .HasColumnName("response_message")
            .HasMaxLength(1000);

        builder.Property(x => x.RawResponse)
            .HasColumnName("raw_response")
            .HasColumnType("json");

        builder.Property(x => x.RespondedAt)
            .HasColumnName("responded_at")
            .HasColumnType("datetime(6)")
            .IsRequired();

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

        builder.HasOne(x => x.EligibilityRequest)
            .WithOne(x => x.Response)
            .HasForeignKey<EligibilityResponse>(
                x => x.EligibilityRequestId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.Benefits)
            .WithOne(x => x.EligibilityResponse)
            .HasForeignKey(x => x.EligibilityResponseId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}