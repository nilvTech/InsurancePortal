
using InsureFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InsureFlow.Infrastructure.Persistence.Configurations;

public sealed class AppealExternalReviewConfiguration
    : IEntityTypeConfiguration<AppealExternalReview>
{
    public void Configure(
        EntityTypeBuilder<AppealExternalReview> builder)
    {
        builder.ToTable("appeal_external_reviews");

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

        builder.Property(x => x.ReviewingOrganization)
            .HasColumnName("reviewing_organization")
            .HasMaxLength(250)
            .IsRequired();

        builder.Property(x => x.ExternalReferenceNumber)
            .HasColumnName("external_reference_number")
            .HasMaxLength(100);

        builder.Property(x => x.Status)
            .HasColumnName("status")
            .HasMaxLength(30)
            .IsRequired();

        builder.Property(x => x.RequestedAtUtc)
            .HasColumnName("requested_at_utc")
            .HasColumnType("datetime(6)")
            .IsRequired();

        builder.Property(x => x.ReviewDueAtUtc)
            .HasColumnName("review_due_at_utc")
            .HasColumnType("datetime(6)");

        builder.Property(x => x.RespondedAtUtc)
            .HasColumnName("responded_at_utc")
            .HasColumnType("datetime(6)");

        builder.Property(x => x.ReviewNotes)
            .HasColumnName("review_notes")
            .HasMaxLength(4000);

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
            x.AppealId
        });

        builder.HasIndex(x => new
        {
            x.TenantId,
            x.Status
        });

        builder.HasOne<Appeal>()
            .WithMany()
            .HasForeignKey(x => x.AppealId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
