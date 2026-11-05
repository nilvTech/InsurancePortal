using InsureFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InsureFlow.Infrastructure.Persistence.Configurations;

public sealed class ClaimHistoryConfiguration
    : IEntityTypeConfiguration<ClaimHistory>
{
    public void Configure(
        EntityTypeBuilder<ClaimHistory> builder)
    {
        builder.ToTable("claim_history");

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

        builder.Property(x => x.FromStatus)
            .HasColumnName("from_status")
            .HasMaxLength(30)
            .IsRequired();

        builder.Property(x => x.ToStatus)
            .HasColumnName("to_status")
            .HasMaxLength(30)
            .IsRequired();

        builder.Property(x => x.Note)
            .HasColumnName("note")
            .HasColumnType("text");

        builder.Property(x => x.ChangedAt)
            .HasColumnName("changed_at")
            .HasColumnType("datetime(6)")
            .IsRequired();

        builder.Property(x => x.ChangedByUserId)
            .HasColumnName("changed_by_user_id")
            .HasColumnType("bigint unsigned");

        builder.HasIndex(x => new
        {
            x.TenantId,
            x.ClaimId,
            x.ChangedAt
        })
            .HasDatabaseName("ix_claim_history_tenant_claim_changed_at");

        builder.HasIndex(x => new
        {
            x.ClaimId,
            x.Id
        })
            .HasDatabaseName("ix_claim_history_claim_id_id");
    }
}