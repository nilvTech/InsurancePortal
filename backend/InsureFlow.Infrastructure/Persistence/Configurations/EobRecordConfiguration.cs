
using InsureFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InsureFlow.Infrastructure.Persistence.Configurations;

public class EobRecordConfiguration : IEntityTypeConfiguration<EobRecord>
{
    public void Configure(EntityTypeBuilder<EobRecord> builder)
    {
        builder.ToTable("eob_records");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id).HasColumnName("id").HasColumnType("bigint unsigned").ValueGeneratedOnAdd();
        builder.Property(x => x.ExternalId).HasColumnName("eob_guid").HasColumnType("char(36)").IsRequired();
        builder.Property(x => x.TenantId).HasColumnName("tenant_id").HasColumnType("bigint unsigned").IsRequired();
        builder.Property(x => x.PayerId).HasColumnName("payer_id").HasColumnType("bigint unsigned").IsRequired();
        builder.Property(x => x.PatientId).HasColumnName("patient_id").HasColumnType("bigint unsigned");
        builder.Property(x => x.ClaimId).HasColumnName("claim_id").HasColumnType("bigint unsigned");
        builder.Property(x => x.DocumentNumber).HasColumnName("document_number").HasMaxLength(100);
        builder.Property(x => x.FileName).HasColumnName("file_name").HasMaxLength(255);
        builder.Property(x => x.ContentType).HasColumnName("content_type").HasMaxLength(150);
        builder.Property(x => x.StorageReference).HasColumnName("storage_reference").HasMaxLength(1000);
        builder.Property(x => x.FileSizeBytes).HasColumnName("file_size_bytes");
        builder.Property(x => x.ReceivedAtUtc).HasColumnName("received_at_utc").HasColumnType("datetime(6)");
        builder.Property(x => x.StatementDateUtc).HasColumnName("statement_date_utc").HasColumnType("datetime(6)");
        builder.Property(x => x.Status).HasColumnName("status").HasMaxLength(30).IsRequired();
        builder.Property(x => x.Notes).HasColumnName("notes").HasMaxLength(2000);
        builder.Property(x => x.CreatedAtUtc).HasColumnName("created_at_utc").HasColumnType("datetime(6)");
        builder.Property(x => x.UpdatedAtUtc).HasColumnName("updated_at_utc").HasColumnType("datetime(6)");

        builder.HasIndex(x => x.ExternalId).IsUnique();
        builder.HasIndex(x => new { x.TenantId, x.PayerId, x.ReceivedAtUtc });
        builder.HasIndex(x => new { x.TenantId, x.ClaimId });
    }
}
