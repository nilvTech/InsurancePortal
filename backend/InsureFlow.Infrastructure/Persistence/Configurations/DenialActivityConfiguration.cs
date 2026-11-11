
using InsureFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InsureFlow.Infrastructure.Persistence.Configurations;

public sealed class DenialActivityConfiguration
    : IEntityTypeConfiguration<DenialActivity>
{
    public void Configure(EntityTypeBuilder<DenialActivity> builder)
    {
        builder.ToTable("denial_activities");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnName("id")
            .ValueGeneratedOnAdd();

        builder.Property(x => x.ExternalId)
            .HasColumnName("activity_guid")
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

        builder.Property(x => x.ActivityType)
            .HasColumnName("activity_type")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.Description)
            .HasColumnName("description")
            .HasMaxLength(2000)
            .IsRequired();

        builder.Property(x => x.PerformedByUserId)
            .HasColumnName("performed_by_user_id");

        builder.Property(x => x.ActivityDateUtc)
            .HasColumnName("activity_date_utc")
            .HasColumnType("datetime(6)")
            .IsRequired();

        builder.Property(x => x.CreatedAtUtc)
            .HasColumnName("created_at_utc")
            .HasColumnType("datetime(6)")
            .IsRequired();

        builder.Property(x => x.UpdatedAtUtc)
            .HasColumnName("updated_at_utc")
            .HasColumnType("datetime(6)");

        builder.Ignore(x => x.CreatedByUserId);
        builder.Ignore(x => x.UpdatedByUserId);

        builder.HasOne<Denial>()
            .WithMany()
            .HasForeignKey(x => x.DenialId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Tenant>()
            .WithMany()
            .HasForeignKey(x => x.TenantId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => new { x.TenantId, x.DenialId, x.ActivityDateUtc });
    }
}
