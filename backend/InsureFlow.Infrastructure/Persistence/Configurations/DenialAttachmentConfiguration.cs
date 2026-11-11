
using InsureFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InsureFlow.Infrastructure.Persistence.Configurations;

public sealed class DenialAttachmentConfiguration
    : IEntityTypeConfiguration<DenialAttachment>
{
    public void Configure(EntityTypeBuilder<DenialAttachment> builder)
    {
        builder.ToTable("denial_attachments");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnName("id")
            .ValueGeneratedOnAdd();

        builder.Property(x => x.ExternalId)
            .HasColumnName("attachment_guid")
            .HasColumnType("char(36)")
            .IsRequired();

        builder.HasIndex(x => x.ExternalId).IsUnique();

        builder.Property(x => x.TenantId)
            .HasColumnName("tenant_id")
            .IsRequired();

        builder.Property(x => x.DenialId)
            .HasColumnName("denial_id")
            .IsRequired();

        builder.Property(x => x.FileName)
            .HasColumnName("file_name")
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(x => x.ContentType)
            .HasColumnName("content_type")
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(x => x.FileSizeBytes)
            .HasColumnName("file_size_bytes")
            .IsRequired();

        builder.Property(x => x.StorageReference)
            .HasColumnName("storage_reference")
            .HasMaxLength(1000)
            .IsRequired();

        builder.Property(x => x.UploadedByUserId)
            .HasColumnName("uploaded_by_user_id");

        builder.Property(x => x.UploadedAtUtc)
            .HasColumnName("uploaded_at_utc")
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

        builder.HasIndex(x => new { x.TenantId, x.DenialId });
    }
}
