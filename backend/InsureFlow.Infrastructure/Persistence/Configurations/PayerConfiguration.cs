using InsureFlow.Domain.Entities;
using InsureFlow.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InsureFlow.Infrastructure.Persistence.Configurations;

public sealed class PayerConfiguration
    : IEntityTypeConfiguration<Payer>
{
    public void Configure(
        EntityTypeBuilder<Payer> builder)
    {
        builder.ToTable("payers");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnName("id")
            .HasColumnType("bigint unsigned")
            .ValueGeneratedOnAdd();

        builder.Property(x => x.ExternalId)
            .HasColumnName("payer_uuid")
            .HasColumnType("char(36)")
            .IsRequired();

        builder.HasIndex(x => x.ExternalId)
            .IsUnique()
            .HasDatabaseName(
                "uk_payers_payer_uuid");

        builder.Property(x => x.TenantId)
            .HasColumnName("tenant_id")
            .HasColumnType("bigint unsigned")
            .IsRequired();

        builder.HasIndex(x => new
        {
            x.TenantId,
            x.Code
        })
        .IsUnique()
        .HasDatabaseName("uk_payers_1");

        builder.Property(x => x.Code)
            .HasColumnName("payer_code")
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.Name)
            .HasColumnName("payer_name")
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.PayerId)
            .HasColumnName("payer_id")
            .HasMaxLength(50);

        builder.Property(x => x.Type)
            .HasColumnName("payer_type")
            .HasColumnType(
                "enum('COMMERCIAL','MEDICARE','MEDICAID'," +
                "'TRICARE','WORKERS_COMP','OTHER')")
            .HasConversion(
                value => value.ToString().ToUpperInvariant(),
                value => Enum.Parse<PayerType>(
                    value,
                    true))
            .IsRequired();

        builder.Property(x => x.Phone)
            .HasColumnName("phone")
            .HasMaxLength(30);

        builder.Property(x => x.Website)
            .HasColumnName("website")
            .HasMaxLength(500);

        builder.Property(x => x.Status)
            .HasColumnName("status")
            .HasColumnType(
                "enum('ACTIVE','INACTIVE')")
            .HasConversion(
                value => value == RecordStatus.Active
                    ? "ACTIVE"
                    : "INACTIVE",
                value => value == "ACTIVE"
                    ? RecordStatus.Active
                    : RecordStatus.Inactive)
            .IsRequired();

        builder.Property(x => x.CreatedAtUtc)
            .HasColumnName("created_at")
            .HasColumnType("datetime(6)")
            .IsRequired();

        builder.Property(x => x.UpdatedAtUtc)
            .HasColumnName("updated_at")
            .HasColumnType("datetime(6)");

        builder.Ignore(x => x.CreatedByUserId);
        builder.Ignore(x => x.UpdatedByUserId);

        builder.HasOne(x => x.Tenant)
            .WithMany()
            .HasForeignKey(x => x.TenantId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName(
                "fk_payers_tenant_id_1");
    }
}