using InsureFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InsureFlow.Infrastructure.Persistence.Configurations;

public sealed class AuthorizationDocumentConfiguration
    : IEntityTypeConfiguration<AuthorizationDocument>
{
    public void Configure(EntityTypeBuilder<AuthorizationDocument> b)
    {
        b.ToTable("authorization_documents");

        b.HasKey(x => x.Id);

        b.Property(x => x.Id)
            .HasColumnName("id")
            .HasColumnType("bigint unsigned")
            .ValueGeneratedOnAdd();

        b.Property(x => x.TenantId)
            .HasColumnName("tenant_id")
            .HasColumnType("bigint unsigned")
            .IsRequired();

        b.Property(x => x.AuthorizationId)
            .HasColumnName("authorization_id")
            .HasColumnType("bigint unsigned")
            .IsRequired();

        b.Property(x => x.FileName)
            .HasColumnName("file_name")
            .HasMaxLength(255)
            .IsRequired();

        b.Property(x => x.StorageKey)
            .HasColumnName("storage_key")
            .HasMaxLength(500)
            .IsRequired();

        b.Property(x => x.ContentType)
            .HasColumnName("content_type")
            .HasMaxLength(100);

        b.Property(x => x.FileSize)
            .HasColumnName("file_size")
            .HasColumnType("bigint unsigned");

        b.Property(x => x.DocumentType)
            .HasColumnName("document_type")
            .HasMaxLength(40)
            .IsRequired();

        b.Property(x => x.UploadedAt)
            .HasColumnName("uploaded_at")
            .HasColumnType("datetime(6)")
            .IsRequired();

        b.HasIndex(x => new
        {
            x.TenantId,
            x.AuthorizationId
        });
    }
}