using InsureFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InsureFlow.Infrastructure.Persistence.Configurations;

public sealed class EligibilityHistoryConfiguration
    : IEntityTypeConfiguration<EligibilityHistory>
{
    public void Configure(EntityTypeBuilder<EligibilityHistory> builder)
    {
        builder.ToTable("eligibility_history");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnName("id")
            .ValueGeneratedOnAdd();

        builder.Property(x => x.ExternalId)
            .HasColumnName("history_uuid")
            .HasMaxLength(36)
            .IsRequired();

        builder.HasIndex(x => x.ExternalId)
            .IsUnique()
            .HasDatabaseName("uk_eligibility_history_history_uuid");

        builder.Property(x => x.TenantId)
            .HasColumnName("tenant_id")
            .IsRequired();

        builder.HasIndex(x => x.TenantId)
            .HasDatabaseName("idx_eligibility_history_tenant_id");

        builder.Property(x => x.EligibilityRequestId)
            .HasColumnName("eligibility_request_id")
            .IsRequired();

        builder.HasIndex(x => x.EligibilityRequestId)
            .HasDatabaseName("idx_eligibility_history_eligibility_request_id");

        builder.Property(x => x.FromStatus)
            .HasColumnName("from_status")
            .HasMaxLength(30)
            .IsRequired();

        builder.Property(x => x.ToStatus)
            .HasColumnName("to_status")
            .HasMaxLength(30)
            .IsRequired();

        builder.Property(x => x.Reason)
            .HasColumnName("reason")
            .HasMaxLength(500);

        builder.Property(x => x.ChangedAt)
            .HasColumnName("changed_at")
            .HasColumnType("datetime(6)")
            .IsRequired();

        builder.Property(x => x.ChangedByUserId)
            .HasColumnName("changed_by_user_id");

        builder.HasOne(x => x.EligibilityRequest)
            .WithMany(x => x.History)
            .HasForeignKey(x => x.EligibilityRequestId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}