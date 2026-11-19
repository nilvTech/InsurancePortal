
using InsureFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InsureFlow.Infrastructure.Persistence.Configurations;

public sealed class AppealHistoryConfiguration
    : IEntityTypeConfiguration<AppealHistory>
{
    public void Configure(EntityTypeBuilder<AppealHistory> builder)
    {
        builder.ToTable("appeal_history");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnName("id")
            .ValueGeneratedOnAdd();

        builder.Ignore(x => x.ExternalId);
        builder.Ignore(x => x.CreatedAtUtc);
        builder.Ignore(x => x.UpdatedAtUtc);
        builder.Ignore(x => x.CreatedByUserId);
        builder.Ignore(x => x.UpdatedByUserId);

        builder.Property(x => x.TenantId)
            .HasColumnName("tenant_id")
            .IsRequired();

        builder.Property(x => x.AppealId)
            .HasColumnName("appeal_id")
            .IsRequired();

        // The existing table stores the resulting appeal status.
        builder.Property(x => x.ActivityType)
            .HasColumnName("to_status")
            .HasMaxLength(30)
            .IsRequired();

        // The existing table stores the status-change note.
        builder.Property(x => x.Description)
            .HasColumnName("note")
            .HasMaxLength(1000)
            .IsRequired(false);

        builder.Property(x => x.PerformedByUserId)
            .HasColumnName("changed_by_user_id");

        builder.Property(x => x.ActivityDateUtc)
            .HasColumnName("changed_at")
            .HasColumnType("datetime(6)")
            .IsRequired();

        builder.HasIndex(x => new
        {
            x.TenantId,
            x.AppealId,
            x.ActivityDateUtc
        });

        builder.HasOne<Appeal>()
            .WithMany()
            .HasForeignKey(x => x.AppealId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
