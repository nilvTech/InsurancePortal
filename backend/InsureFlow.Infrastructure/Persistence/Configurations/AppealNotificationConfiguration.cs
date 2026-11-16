
using InsureFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InsureFlow.Infrastructure.Persistence.Configurations;

public sealed class AppealNotificationConfiguration
    : IEntityTypeConfiguration<AppealNotification>
{
    public void Configure(
        EntityTypeBuilder<AppealNotification> builder)
    {
        builder.ToTable("appeal_notifications");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnName("id")
            .ValueGeneratedOnAdd();

        builder.Property(x => x.ExternalId)
            .HasColumnName("external_id")
            .HasColumnType("char(36)")
            .IsRequired();

        builder.HasIndex(x => x.ExternalId).IsUnique();

        builder.Property(x => x.TenantId)
            .HasColumnName("tenant_id")
            .IsRequired();

        builder.Property(x => x.AppealId)
            .HasColumnName("appeal_id")
            .IsRequired();

        builder.Property(x => x.NotificationType)
            .HasColumnName("notification_type")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.Title)
            .HasColumnName("title")
            .HasMaxLength(250)
            .IsRequired();

        builder.Property(x => x.Message)
            .HasColumnName("message")
            .HasMaxLength(2000)
            .IsRequired();

        builder.Property(x => x.RecipientUserId)
            .HasColumnName("recipient_user_id");

        builder.Property(x => x.IsRead)
            .HasColumnName("is_read")
            .IsRequired();

        builder.Property(x => x.ReadAtUtc)
            .HasColumnName("read_at_utc")
            .HasColumnType("datetime(6)");

        builder.Property(x => x.CreatedAtUtc)
            .HasColumnName("created_at_utc")
            .HasColumnType("datetime(6)")
            .IsRequired();

        builder.Property(x => x.UpdatedAtUtc)
            .HasColumnName("updated_at_utc")
            .HasColumnType("datetime(6)");

        builder.Property(x => x.CreatedByUserId)
            .HasColumnName("created_by_user_id");

        builder.Property(x => x.UpdatedByUserId)
            .HasColumnName("updated_by_user_id");

        builder.HasIndex(x => new
        {
            x.TenantId,
            x.AppealId,
            x.IsRead
        });

        builder.HasIndex(x => new
        {
            x.TenantId,
            x.RecipientUserId
        });

        builder.HasOne<Appeal>()
            .WithMany()
            .HasForeignKey(x => x.AppealId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
