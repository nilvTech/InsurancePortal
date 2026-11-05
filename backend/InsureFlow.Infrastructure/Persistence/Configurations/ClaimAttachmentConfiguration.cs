
using InsureFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InsureFlow.Infrastructure.Persistence.Configurations;

public sealed class ClaimAttachmentConfiguration
    : IEntityTypeConfiguration<ClaimAttachment>
{
    public void Configure(EntityTypeBuilder<ClaimAttachment> builder)
    {
        builder.ToTable("claim_attachments");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnName("id")
            .HasColumnType("bigint unsigned")
            .ValueGeneratedOnAdd();

        builder.Property(x => x.TenantId)
            .HasColumnName("tenant_id")
            .HasColumnType("bigint unsigned")
            .IsRequired();

        builder.Property(x => x.ClaimId)
            .HasColumnName("claim_id")
            .HasColumnType("bigint unsigned")
            .IsRequired();

        builder.Property(x => x.FileName)
            .HasColumnName("file_name")
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(x => x.StorageKey)
            .HasColumnName("storage_key")
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(x => x.ContentType)
            .HasColumnName("content_type")
            .HasMaxLength(100);

        builder.Property(x => x.FileSize)
            .HasColumnName("file_size")
            .HasColumnType("bigint unsigned");

        builder.Property(x => x.DocumentType)
            .HasColumnName("document_type")
            .HasMaxLength(40)
            .IsRequired();

        builder.Property(x => x.UploadedAt)
            .HasColumnName("uploaded_at")
            .HasColumnType("datetime(6)")
            .IsRequired();

        builder.HasIndex(x => new
        {
            x.TenantId,
            x.ClaimId
        });
    }
}
