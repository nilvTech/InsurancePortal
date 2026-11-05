using InsureFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InsureFlow.Infrastructure.Persistence.Configurations;

public sealed class EligibilityBenefitConfiguration
    : IEntityTypeConfiguration<EligibilityBenefit>
{
    public void Configure(EntityTypeBuilder<EligibilityBenefit> builder)
    {
        builder.ToTable("eligibility_benefits");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnName("id")
            .ValueGeneratedOnAdd();

        builder.Property(x => x.ExternalId)
            .HasColumnName("benefit_uuid")
            .HasMaxLength(36)
            .IsRequired();

        builder.HasIndex(x => x.ExternalId)
            .IsUnique()
            .HasDatabaseName("uk_eligibility_benefits_benefit_uuid");

        builder.Property(x => x.TenantId)
            .HasColumnName("tenant_id")
            .IsRequired();

        builder.HasIndex(x => x.TenantId)
            .HasDatabaseName("idx_eligibility_benefits_tenant_id");

        builder.Property(x => x.EligibilityResponseId)
            .HasColumnName("eligibility_response_id")
            .IsRequired();

        builder.HasIndex(x => x.EligibilityResponseId)
            .HasDatabaseName("idx_eligibility_benefits_eligibility_response_id");

        builder.Property(x => x.Category)
            .HasColumnName("category")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.ServiceTypeCode)
            .HasColumnName("service_type_code")
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.Description)
            .HasColumnName("description")
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(x => x.CoverageLevel)
            .HasColumnName("coverage_level")
            .HasMaxLength(50);

        builder.Property(x => x.Copay)
            .HasColumnName("copay")
            .HasPrecision(18, 2);

        builder.Property(x => x.CoinsurancePercent)
            .HasColumnName("coinsurance_percent")
            .HasPrecision(5, 2);

        builder.Property(x => x.Deductible)
            .HasColumnName("deductible")
            .HasPrecision(18, 2);

        builder.Property(x => x.Remaining)
            .HasColumnName("remaining")
            .HasPrecision(18, 2);

        builder.Property(x => x.Status)
            .HasColumnName("status")
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.HasIndex(x => x.Status)
            .HasDatabaseName("idx_eligibility_benefits_status");

        builder.HasOne(x => x.EligibilityResponse)
            .WithMany(x => x.Benefits)
            .HasForeignKey(x => x.EligibilityResponseId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}