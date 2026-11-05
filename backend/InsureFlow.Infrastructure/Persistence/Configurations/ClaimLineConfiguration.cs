
using InsureFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InsureFlow.Infrastructure.Persistence.Configurations;

public sealed class ClaimLineConfiguration
    : IEntityTypeConfiguration<ClaimLine>
{
    public void Configure(EntityTypeBuilder<ClaimLine> builder)
    {
        builder.ToTable("claim_lines");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnName("id")
            .ValueGeneratedOnAdd();

        builder.Property(x => x.ExternalId)
            .HasColumnName("claim_line_uuid")
            .HasColumnType("char(36)")
            .HasMaxLength(36)
            .IsRequired();

        builder.HasIndex(x => x.ExternalId)
            .IsUnique()
            .HasDatabaseName("uk_claim_lines_claim_line_uuid");

        builder.Property(x => x.TenantId)
            .HasColumnName("tenant_id")
            .IsRequired();

        builder.HasIndex(x => x.TenantId)
            .HasDatabaseName("idx_claim_lines_tenant_id");

        builder.Property(x => x.ClaimId)
            .HasColumnName("claim_id")
            .IsRequired();

        builder.HasIndex(x => new { x.ClaimId, x.LineNumber })
            .IsUnique()
            .HasDatabaseName("uk_claim_lines_claim_line_number");

        builder.Property(x => x.LineNumber)
            .HasColumnName("line_number")
            .IsRequired();

        builder.Property(x => x.ProcedureCode)
            .HasColumnName("cpt_hcpcs_code")
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(x => x.Modifier1)
            .HasColumnName("modifier1")
            .HasMaxLength(10);

        builder.Property(x => x.Modifier2)
            .HasColumnName("modifier2")
            .HasMaxLength(10);

        builder.Property(x => x.Modifier3)
            .HasColumnName("modifier3")
            .HasMaxLength(10);

        builder.Property(x => x.Modifier4)
            .HasColumnName("modifier4")
            .HasMaxLength(10);

        builder.Property(x => x.PlaceOfService)
            .HasColumnName("place_of_service")
            .HasMaxLength(10);

        builder.Property(x => x.Units)
            .HasColumnName("units")
            .HasColumnType("int")
            .IsRequired();

        builder.Property(x => x.ChargeAmount)
            .HasColumnName("charge_amount")
            .HasColumnType("decimal(12,2)")
            .IsRequired();

        builder.Property(x => x.ServiceDate)
            .HasColumnName("service_date")
            .HasColumnType("date")
            .IsRequired();

        builder.Property(x => x.DiagnosisPointers)
            .HasColumnName("diagnosis_pointer")
            .HasMaxLength(50);

        // The current claim_lines schema has no audit timestamp or user columns.
        builder.Ignore(x => x.CreatedAtUtc);
        builder.Ignore(x => x.UpdatedAtUtc);
        builder.Ignore(x => x.CreatedByUserId);
        builder.Ignore(x => x.UpdatedByUserId);

        builder.HasOne(x => x.Claim)
            .WithMany()
            .HasForeignKey(x => x.ClaimId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<Tenant>()
            .WithMany()
            .HasForeignKey(x => x.TenantId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
