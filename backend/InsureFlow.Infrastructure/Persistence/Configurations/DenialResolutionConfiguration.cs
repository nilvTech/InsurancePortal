
using InsureFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InsureFlow.Infrastructure.Persistence.Configurations;

public sealed class DenialResolutionConfiguration
    : IEntityTypeConfiguration<DenialResolution>
{
    public void Configure(EntityTypeBuilder<DenialResolution> builder)
    {
        builder.ToTable("denial_resolutions");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.ExternalId)
            .HasColumnName("resolution_guid")
            .HasColumnType("char(36)")
            .IsRequired();

        builder.HasIndex(x => x.ExternalId)
            .IsUnique();

        builder.Property(x => x.TenantId)
            .HasColumnName("tenant_id")
            .IsRequired();

        builder.Property(x => x.DenialId)
            .HasColumnName("denial_id")
            .IsRequired();

        builder.Property(x => x.ResolutionType)
            .HasColumnName("resolution_type")
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.ResolutionNotes)
            .HasColumnName("resolution_notes")
            .HasColumnType("text")
            .IsRequired();

        builder.Property(x => x.ResolvedAmount)
            .HasColumnName("resolved_amount")
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(x => x.ResolutionDateUtc)
            .HasColumnName("resolution_date_utc")
            .HasColumnType("datetime(6)")
            .IsRequired();

        builder.Property(x => x.ResolvedByUserId)
            .HasColumnName("resolved_by_user_id");

        builder.Property(x => x.CreatedAtUtc)
            .HasColumnName("created_at_utc")
            .HasColumnType("datetime(6)")
            .IsRequired();

        builder.Property(x => x.UpdatedAtUtc)
            .HasColumnName("updated_at_utc")
            .HasColumnType("datetime(6)");

        builder.HasOne<Denial>()
            .WithMany()
            .HasForeignKey(x => x.DenialId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Tenant>()
            .WithMany()
            .HasForeignKey(x => x.TenantId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => new { x.TenantId, x.DenialId });

        builder.HasIndex(x => new { x.DenialId, x.ResolutionDateUtc });
    }
}
