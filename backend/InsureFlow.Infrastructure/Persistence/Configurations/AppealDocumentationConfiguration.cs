
using InsureFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InsureFlow.Infrastructure.Persistence.Configurations;

public sealed class AppealDocumentationConfiguration
    : IEntityTypeConfiguration<AppealDocumentation>
{
    public void Configure(
        EntityTypeBuilder<AppealDocumentation> builder)
    {
        builder.ToTable("appeal_documentation");

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

        builder.Property(x => x.DocumentTitle)
            .HasColumnName("document_title")
            .HasMaxLength(250)
            .IsRequired();

        builder.Property(x => x.DocumentType)
            .HasColumnName("document_type")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.Description)
            .HasColumnName("description")
            .HasMaxLength(2000);

        builder.Property(x => x.StorageReference)
            .HasColumnName("storage_reference")
            .HasMaxLength(1000);

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

        builder.HasOne<Appeal>()
            .WithMany()
            .HasForeignKey(x => x.AppealId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
