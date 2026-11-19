
using InsureFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InsureFlow.Infrastructure.Persistence.Configurations;

public class PaymentReconciliationConfiguration : IEntityTypeConfiguration<PaymentReconciliation>
{
    public void Configure(EntityTypeBuilder<PaymentReconciliation> builder)
    {
        builder.ToTable("if_payment_reconciliations");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id).HasColumnName("id").HasColumnType("bigint unsigned").ValueGeneratedOnAdd();
        builder.Property(x => x.ExternalId).HasColumnName("reconciliation_guid").HasColumnType("char(36)").IsRequired();
        builder.Property(x => x.TenantId).HasColumnName("tenant_id").HasColumnType("bigint unsigned").IsRequired();
        builder.Property(x => x.ReconciliationNumber).HasColumnName("reconciliation_number").HasMaxLength(100).IsRequired();
        builder.Property(x => x.PeriodStartUtc).HasColumnName("period_start_utc").HasColumnType("datetime(6)");
        builder.Property(x => x.PeriodEndUtc).HasColumnName("period_end_utc").HasColumnType("datetime(6)");
        builder.Property(x => x.StartedAtUtc).HasColumnName("started_at_utc").HasColumnType("datetime(6)");
        builder.Property(x => x.CompletedAtUtc).HasColumnName("completed_at_utc").HasColumnType("datetime(6)");
        builder.Property(x => x.ExpectedAmount).HasColumnName("expected_amount").HasPrecision(18, 2);
        builder.Property(x => x.ActualAmount).HasColumnName("actual_amount").HasPrecision(18, 2);
        builder.Property(x => x.DifferenceAmount).HasColumnName("difference_amount").HasPrecision(18, 2);
        builder.Property(x => x.PaymentsProcessed).HasColumnName("payments_processed");
        builder.Property(x => x.PaymentsMatched).HasColumnName("payments_matched");
        builder.Property(x => x.PaymentsUnmatched).HasColumnName("payments_unmatched");
        builder.Property(x => x.Status).HasColumnName("status").HasMaxLength(30).IsRequired();
        builder.Property(x => x.Summary).HasColumnName("summary").HasMaxLength(4000);
        builder.Property(x => x.ErrorMessage).HasColumnName("error_message").HasMaxLength(4000);
        builder.Property(x => x.CreatedAtUtc).HasColumnName("created_at_utc").HasColumnType("datetime(6)");
        builder.Property(x => x.UpdatedAtUtc).HasColumnName("updated_at_utc").HasColumnType("datetime(6)");

        builder.HasIndex(x => x.ExternalId).IsUnique();
        builder.HasIndex(x => new { x.TenantId, x.ReconciliationNumber }).IsUnique();
        builder.HasIndex(x => new { x.TenantId, x.StartedAtUtc });
    }
}
