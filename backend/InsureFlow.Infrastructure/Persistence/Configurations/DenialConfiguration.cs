
using InsureFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InsureFlow.Infrastructure.Persistence.Configurations;

public sealed class DenialConfiguration : IEntityTypeConfiguration<Denial>
{
    public void Configure(EntityTypeBuilder<Denial> builder)
    {
        builder.ToTable("denials");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnName("id")
            .ValueGeneratedOnAdd();

        builder.Property(x => x.ExternalId)
            .HasColumnName("denial_guid")
            .HasColumnType("char(36)")
            .IsRequired();

        builder.HasIndex(x => x.ExternalId)
            .IsUnique();

        builder.Property(x => x.TenantId)
            .HasColumnName("tenant_id")
            .IsRequired();

        builder.Property(x => x.ClaimId)
            .HasColumnName("claim_id")
            .IsRequired();

        builder.Property(x => x.DenialNumber)
            .HasColumnName("denial_number")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.DenialCode)
            .HasColumnName("denial_code")
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.DenialReason)
            .HasColumnName("denial_reason")
            .HasMaxLength(1000)
            .IsRequired();

        builder.Property(x => x.DenialCategory)
            .HasColumnName("denial_category")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.DeniedAmount)
            .HasColumnName("denied_amount")
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(x => x.DenialDate)
            .HasColumnName("denial_date")
            .HasColumnType("datetime(6)")
            .IsRequired();

        builder.Property(x => x.AppealDeadline)
            .HasColumnName("appeal_deadline")
            .HasColumnType("datetime(6)");

        builder.Property(x => x.Status)
            .HasColumnName("status")
            .HasMaxLength(30)
            .IsRequired();

        builder.Property(x => x.AssignedToUserId)
            .HasColumnName("assigned_to_user_id");

        builder.Property(x => x.CreatedAtUtc)
            .HasColumnName("created_at_utc")
            .HasColumnType("datetime(6)")
            .IsRequired();

        builder.Property(x => x.UpdatedAtUtc)
            .HasColumnName("updated_at_utc")
            .HasColumnType("datetime(6)");

        builder.Ignore(x => x.CreatedByUserId);
        builder.Ignore(x => x.UpdatedByUserId);

        builder.HasOne<Tenant>()
            .WithMany()
            .HasForeignKey(x => x.TenantId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Claim>()
            .WithMany()
            .HasForeignKey(x => x.ClaimId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => new { x.TenantId, x.DenialNumber })
            .IsUnique();

        builder.HasIndex(x => new { x.TenantId, x.ClaimId });

        builder.HasIndex(x => new { x.TenantId, x.Status });

        builder.HasIndex(x => new { x.TenantId, x.DenialDate });
    }
}
