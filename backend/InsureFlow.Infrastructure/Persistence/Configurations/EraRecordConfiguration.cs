
using InsureFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InsureFlow.Infrastructure.Persistence.Configurations;

public class EraRecordConfiguration : IEntityTypeConfiguration<EraRecord>
{
    public void Configure(EntityTypeBuilder<EraRecord> builder)
    {
        builder.ToTable("era_records");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id).HasColumnName("id").HasColumnType("bigint unsigned").ValueGeneratedOnAdd();
        builder.Property(x => x.ExternalId).HasColumnName("era_guid").HasColumnType("char(36)").IsRequired();
        builder.Property(x => x.TenantId).HasColumnName("tenant_id").HasColumnType("bigint unsigned").IsRequired();
        builder.Property(x => x.PayerId).HasColumnName("payer_id").HasColumnType("bigint unsigned").IsRequired();
        builder.Property(x => x.TraceNumber).HasColumnName("trace_number").HasMaxLength(100);
        builder.Property(x => x.CheckNumber).HasColumnName("check_number").HasMaxLength(100);
        builder.Property(x => x.TotalPaymentAmount).HasColumnName("total_payment_amount").HasPrecision(18, 2);
        builder.Property(x => x.PaymentDateUtc).HasColumnName("payment_date_utc").HasColumnType("datetime(6)");
        builder.Property(x => x.ReceivedAtUtc).HasColumnName("received_at_utc").HasColumnType("datetime(6)");
        builder.Property(x => x.FileName).HasColumnName("file_name").HasMaxLength(255);
        builder.Property(x => x.StorageReference).HasColumnName("storage_reference").HasMaxLength(1000);
        builder.Property(x => x.TransactionControlNumber).HasColumnName("transaction_control_number").HasMaxLength(100);
        builder.Property(x => x.ImportStatus).HasColumnName("import_status").HasMaxLength(30).IsRequired();
        builder.Property(x => x.ErrorMessage).HasColumnName("error_message").HasMaxLength(4000);
        builder.Property(x => x.RawContent).HasColumnName("raw_content").HasColumnType("longtext");
        builder.Property(x => x.CreatedAtUtc).HasColumnName("created_at_utc").HasColumnType("datetime(6)");
        builder.Property(x => x.UpdatedAtUtc).HasColumnName("updated_at_utc").HasColumnType("datetime(6)");

        builder.HasIndex(x => x.ExternalId).IsUnique();
        builder.HasIndex(x => new { x.TenantId, x.PayerId, x.ReceivedAtUtc });
        builder.HasIndex(x => new { x.TenantId, x.TraceNumber });
    }
}
