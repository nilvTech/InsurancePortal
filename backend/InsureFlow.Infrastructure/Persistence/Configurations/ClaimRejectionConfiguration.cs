using InsureFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InsureFlow.Infrastructure.Persistence.Configurations;

public sealed class ClaimRejectionConfiguration
    : IEntityTypeConfiguration<ClaimRejection>
{
    public void Configure(EntityTypeBuilder<ClaimRejection> builder)
    {
        builder.ToTable("claim_rejections");

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

        builder.Property(x => x.RejectionCode)
            .HasColumnName("rejection_code")
            .HasMaxLength(30)
            .IsRequired();

        builder.Property(x => x.RejectionCategory)
            .HasColumnName("rejection_category")
            .HasMaxLength(50);

        builder.Property(x => x.Description)
            .HasColumnName("description")
            .HasMaxLength(1000)
            .IsRequired();

        builder.Property(x => x.Status)
            .HasColumnName("status")
            .HasMaxLength(30)
            .HasDefaultValue("OPEN")
            .IsRequired();

        builder.Property(x => x.RejectedAt)
            .HasColumnName("rejected_at")
            .HasColumnType("datetime(6)")
            .IsRequired();

        builder.Property(x => x.ResolvedAt)
            .HasColumnName("resolved_at")
            .HasColumnType("datetime(6)");

        builder.Property(x => x.ResolutionNote)
            .HasColumnName("resolution_note")
            .HasMaxLength(1000);

        builder.HasIndex(x => x.TenantId)
            .HasDatabaseName("idx_claim_rejections_tenant_id");

        builder.HasIndex(x => x.Status)
            .HasDatabaseName("idx_claim_rejections_status");

        builder.HasIndex(x => x.RejectedAt)
            .HasDatabaseName("idx_claim_rejections_rejected_at");

        builder.HasIndex(x => x.ClaimId)
            .HasDatabaseName("fk_claim_rejections_claim_id_1");

        builder.HasOne<Claim>()
            .WithMany()
            .HasForeignKey(x => x.ClaimId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}