
using InsureFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InsureFlow.Infrastructure.Persistence.Configurations;

public class RefundConfiguration : IEntityTypeConfiguration<Refund>
{
    public void Configure(EntityTypeBuilder<Refund> builder)
    {
        builder.ToTable("refunds");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id).HasColumnName("id").HasColumnType("bigint unsigned").ValueGeneratedOnAdd();
        builder.Property(x => x.ExternalId).HasColumnName("refund_guid").HasColumnType("char(36)").IsRequired();
        builder.Property(x => x.TenantId).HasColumnName("tenant_id").HasColumnType("bigint unsigned").IsRequired();
        builder.Property(x => x.PaymentId).HasColumnName("payment_id").HasColumnType("bigint unsigned").IsRequired();
        builder.Property(x => x.RefundNumber).HasColumnName("refund_number").HasMaxLength(100).IsRequired();
        builder.Property(x => x.RefundAmount).HasColumnName("refund_amount").HasPrecision(18, 2);
        builder.Property(x => x.Reason).HasColumnName("reason").HasMaxLength(1000).IsRequired();
        builder.Property(x => x.Status).HasColumnName("status").HasMaxLength(30).IsRequired();
        builder.Property(x => x.RequestedAtUtc).HasColumnName("requested_at_utc").HasColumnType("datetime(6)");
        builder.Property(x => x.RequestedByUserId).HasColumnName("requested_by_user_id").HasColumnType("bigint unsigned");
        builder.Property(x => x.ReviewedAtUtc).HasColumnName("reviewed_at_utc").HasColumnType("datetime(6)");
        builder.Property(x => x.ReviewedByUserId).HasColumnName("reviewed_by_user_id").HasColumnType("bigint unsigned");
        builder.Property(x => x.ReviewNotes).HasColumnName("review_notes").HasMaxLength(2000);
        builder.Property(x => x.ProcessedAtUtc).HasColumnName("processed_at_utc").HasColumnType("datetime(6)");
        builder.Property(x => x.CreatedAtUtc).HasColumnName("created_at_utc").HasColumnType("datetime(6)");
        builder.Property(x => x.UpdatedAtUtc).HasColumnName("updated_at_utc").HasColumnType("datetime(6)");

        builder.HasIndex(x => x.ExternalId).IsUnique();
        builder.HasIndex(x => new { x.TenantId, x.RefundNumber }).IsUnique();
        builder.HasIndex(x => new { x.TenantId, x.PaymentId });
        builder.HasIndex(x => new { x.TenantId, x.Status });

        builder.HasOne<Payment>()
            .WithMany()
            .HasForeignKey(x => x.PaymentId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
