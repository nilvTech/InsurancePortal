
using InsureFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InsureFlow.Infrastructure.Persistence.Configurations;

public sealed class AppealConfiguration
    : IEntityTypeConfiguration<Appeal>
{
    public void Configure(EntityTypeBuilder<Appeal> builder)
    {
        builder.ToTable("appeals");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnName("id")
            .ValueGeneratedOnAdd();

        builder.Property(x => x.ExternalId)
            .HasColumnName("external_id")
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

        builder.Property(x => x.AppealNumber)
            .HasColumnName("appeal_number")
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.AppealLevel)
            .HasColumnName("appeal_level")
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.AppealReason)
            .HasColumnName("appeal_reason")
            .HasMaxLength(2000)
            .IsRequired();

        builder.Property(x => x.AppealNotes)
            .HasColumnName("appeal_notes")
            .HasMaxLength(4000);

        builder.Property(x => x.Status)
            .HasColumnName("status")
            .HasMaxLength(30)
            .IsRequired();

        builder.Property(x => x.AppealDeadlineUtc)
            .HasColumnName("appeal_deadline_utc")
            .HasColumnType("datetime(6)");

        builder.Property(x => x.SubmittedAtUtc)
            .HasColumnName("submitted_at_utc")
            .HasColumnType("datetime(6)");

        builder.Property(x => x.WithdrawnAtUtc)
            .HasColumnName("withdrawn_at_utc")
            .HasColumnType("datetime(6)");

        builder.Property(x => x.Decision)
            .HasColumnName("decision")
            .HasMaxLength(30);

        builder.Property(x => x.DecisionNotes)
            .HasColumnName("decision_notes")
            .HasMaxLength(4000);

        builder.Property(x => x.DecisionAtUtc)
            .HasColumnName("decision_at_utc")
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
            x.AppealNumber
        }).IsUnique();

        builder.HasIndex(x => new
        {
            x.TenantId,
            x.DenialId
        });

        builder.HasIndex(x => new
        {
            x.TenantId,
            x.Status
        });

        builder.HasOne<Denial>()
            .WithMany()
            .HasForeignKey(x => x.DenialId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
