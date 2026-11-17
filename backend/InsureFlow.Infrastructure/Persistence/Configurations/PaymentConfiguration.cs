
using InsureFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InsureFlow.Infrastructure.Persistence.Configurations;

public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
{
    public void Configure(EntityTypeBuilder<Payment> builder)
    {
        builder.ToTable("payments");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id").HasColumnType("bigint unsigned").ValueGeneratedOnAdd();
        builder.Property(x => x.ExternalId).HasColumnName("payment_guid").HasColumnType("char(36)").IsRequired();
        builder.Property(x => x.TenantId).HasColumnName("tenant_id").HasColumnType("bigint unsigned").IsRequired();
        builder.Property(x => x.PaymentNumber).HasColumnName("payment_number").HasMaxLength(100).IsRequired();
        builder.Property(x => x.PayerId).HasColumnName("payer_id").HasColumnType("bigint unsigned").IsRequired();
        builder.Property(x => x.PatientId).HasColumnName("patient_id").HasColumnType("bigint unsigned");
        builder.Property(x => x.PatientInsuranceId).HasColumnName("patient_insurance_id").HasColumnType("bigint unsigned");
        builder.Property(x => x.CheckNumber).HasColumnName("check_number").HasMaxLength(100);
        builder.Property(x => x.TraceNumber).HasColumnName("trace_number").HasMaxLength(100);
        builder.Property(x => x.PaymentMethod).HasColumnName("payment_method").HasMaxLength(30).IsRequired();
        builder.Property(x => x.PaymentAmount).HasColumnName("payment_amount").HasPrecision(18, 2);
        builder.Property(x => x.AllocatedAmount).HasColumnName("allocated_amount").HasPrecision(18, 2);
        builder.Property(x => x.UnallocatedAmount).HasColumnName("unallocated_amount").HasPrecision(18, 2);
        builder.Property(x => x.Currency).HasColumnName("currency").HasMaxLength(3).IsRequired();
        builder.Property(x => x.PaymentDateUtc).HasColumnName("payment_date_utc").HasColumnType("datetime(6)");
        builder.Property(x => x.ReceivedDateUtc).HasColumnName("received_date_utc").HasColumnType("datetime(6)");
        builder.Property(x => x.PostedAtUtc).HasColumnName("posted_at_utc").HasColumnType("datetime(6)");
        builder.Property(x => x.ReversedAtUtc).HasColumnName("reversed_at_utc").HasColumnType("datetime(6)");
        builder.Property(x => x.Status).HasColumnName("status").HasMaxLength(30).IsRequired();
        builder.Property(x => x.Notes).HasColumnName("notes").HasMaxLength(2000);
        builder.Property(x => x.CreatedAtUtc).HasColumnName("created_at_utc").HasColumnType("datetime(6)");
        builder.Property(x => x.UpdatedAtUtc).HasColumnName("updated_at_utc").HasColumnType("datetime(6)");
        builder.HasIndex(x => x.ExternalId).IsUnique();
        builder.HasIndex(x => new { x.TenantId, x.PaymentNumber }).IsUnique();
        builder.HasIndex(x => new { x.TenantId, x.PayerId, x.PaymentDateUtc });
        builder.HasIndex(x => new { x.TenantId, x.Status });
        builder.HasOne<Payer>()
            .WithMany()
            .HasForeignKey(x => x.PayerId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
