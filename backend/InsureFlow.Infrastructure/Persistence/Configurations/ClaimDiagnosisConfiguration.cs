
using InsureFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InsureFlow.Infrastructure.Persistence.Configurations;

public sealed class ClaimDiagnosisConfiguration
    : IEntityTypeConfiguration<ClaimDiagnosis>
{
    public void Configure(EntityTypeBuilder<ClaimDiagnosis> builder)
    {
        builder.ToTable("claim_diagnoses");

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

        builder.Property(x => x.Icd10Code)
            .HasColumnName("icd10_code")
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(x => x.Description)
            .HasColumnName("description")
            .HasMaxLength(500);

        builder.Property(x => x.Sequence)
            .HasColumnName("sequence")
            .IsRequired();

        builder.HasIndex(x => new
        {
            x.TenantId,
            x.ClaimId,
            x.Sequence
        });
    }
}
