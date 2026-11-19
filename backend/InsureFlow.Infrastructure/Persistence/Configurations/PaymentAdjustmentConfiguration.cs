
using InsureFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InsureFlow.Infrastructure.Persistence.Configurations;

public class PaymentAdjustmentConfiguration : IEntityTypeConfiguration<PaymentAdjustment>
{
    public void Configure(EntityTypeBuilder<PaymentAdjustment> builder)
    {
        builder.ToTable("if_payment_adjustments");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id).HasColumnName("id").HasColumnType("bigint unsigned").ValueGeneratedOnAdd();
        builder.Property(x => x.ExternalId).HasColumnName("adjustment_guid").HasColumnType("char(36)").IsRequired();
        builder.Property(x => x.TenantId).HasColumnName("tenant_id").HasColumnType("bigint unsigned").IsRequired();
        builder.Property(x => x.PaymentId).HasColumnName("payment_id").HasColumnType("bigint unsigned").IsRequired();
        builder.Property(x => x.PaymentAllocationId).HasColumnName("payment_allocation_id").HasColumnType("bigint unsigned");
        builder.Property(x => x.ClaimId).HasColumnName("claim_id").HasColumnType("bigint unsigned");
        builder.Property(x => x.AdjustmentCode).HasColumnName("adjustment_code").HasMaxLength(30).IsRequired();
        builder.Property(x => x.AdjustmentGroupCode).HasColumnName("adjustment_group_code").HasMaxLength(30);
        builder.Property(x => x.Description).HasColumnName("description").HasMaxLength(500).IsRequired();
        builder.Property(x => x.AdjustmentAmount).HasColumnName("adjustment_amount").HasPrecision(18, 2);
        builder.Property(x => x.AdjustmentDateUtc).HasColumnName("adjustment_date_utc").HasColumnType("datetime(6)");
        builder.Property(x => x.Status).HasColumnName("status").HasMaxLength(30).IsRequired();
        builder.Property(x => x.Notes).HasColumnName("notes").HasMaxLength(2000);
        builder.Property(x => x.CreatedAtUtc).HasColumnName("created_at_utc").HasColumnType("datetime(6)");
        builder.Property(x => x.UpdatedAtUtc).HasColumnName("updated_at_utc").HasColumnType("datetime(6)");
        builder.HasIndex(x => x.ExternalId).IsUnique();
        builder.HasIndex(x => new { x.TenantId, x.PaymentId });
        builder.HasIndex(x => new { x.TenantId, x.ClaimId });
    }
}
