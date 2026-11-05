using InsureFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InsureFlow.Infrastructure.Persistence.Configurations;

public sealed class InsurancePlanConfiguration
    : IEntityTypeConfiguration<InsurancePlan>
{
    public void Configure(
        EntityTypeBuilder<InsurancePlan> builder)
    {
        builder.ToTable("insurance_plans");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnName("id");

        builder.Property(x => x.ExternalId)
            .HasColumnName("insurance_plan_uuid")
            .IsRequired();

        builder.Property(x => x.TenantId)
            .HasColumnName("tenant_id")
            .IsRequired();

        builder.Property(x => x.PayerId)
            .HasColumnName("payer_id")
            .IsRequired();

        builder.Property(x => x.PlanCode)
            .HasColumnName("plan_code")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.PlanName)
            .HasColumnName("plan_name")
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(x => x.PlanType)
            .HasColumnName("plan_type")
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.NetworkType)
            .HasColumnName("network_type")
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(x => x.Status)
            .HasColumnName("status")
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.CreatedAtUtc)
            .HasColumnName("created_at")
            .IsRequired();

        builder.Property(x => x.UpdatedAtUtc)
            .HasColumnName("updated_at");
        builder.Ignore(x => x.CreatedByUserId);
        builder.Ignore(x => x.UpdatedByUserId);
        builder.HasIndex(x => new
        {
            x.TenantId,
            x.PlanCode
        })
            .IsUnique();

        builder.HasOne(x => x.Tenant)
            .WithMany()
            .HasForeignKey(x => x.TenantId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Payer)
            .WithMany()
            .HasForeignKey(x => x.PayerId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}