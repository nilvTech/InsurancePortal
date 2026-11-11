
using InsureFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InsureFlow.Infrastructure.Persistence.Configurations;

public sealed class DenialPayerResponseConfiguration
    : IEntityTypeConfiguration<DenialPayerResponse>
{
    public void Configure(EntityTypeBuilder<DenialPayerResponse> builder)
    {
        builder.ToTable("denial_payer_responses");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.ExternalId)
            .HasColumnName("response_guid")
            .HasColumnType("char(36)")
            .IsRequired();

        builder.HasIndex(x => x.ExternalId)
            .IsUnique();

        builder.Property(x => x.TenantId)
            .HasColumnName("tenant_id")
            .IsRequired();

        builder.Property(x => x.DenialId)
            .HasColumnName("denial_id")
            .IsRequired();

        builder.Property(x => x.ResponseType)
            .HasColumnName("response_type")
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.ResponseCode)
            .HasColumnName("response_code")
            .HasMaxLength(100);

        builder.Property(x => x.ResponseMessage)
            .HasColumnName("response_message")
            .HasColumnType("text")
            .IsRequired();

        builder.Property(x => x.PayerReferenceNumber)
            .HasColumnName("payer_reference_number")
            .HasMaxLength(150);

        builder.Property(x => x.ResponseDateUtc)
            .HasColumnName("response_date_utc")
            .HasColumnType("datetime(6)")
            .IsRequired();

        builder.Property(x => x.ReceivedByUserId)
            .HasColumnName("received_by_user_id");

        builder.Property(x => x.CreatedAtUtc)
            .HasColumnName("created_at_utc")
            .HasColumnType("datetime(6)")
            .IsRequired();

        builder.Property(x => x.UpdatedAtUtc)
            .HasColumnName("updated_at_utc")
            .HasColumnType("datetime(6)");

        builder.HasOne<Denial>()
            .WithMany()
            .HasForeignKey(x => x.DenialId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Tenant>()
            .WithMany()
            .HasForeignKey(x => x.TenantId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => new { x.TenantId, x.DenialId });

        builder.HasIndex(x => new { x.DenialId, x.ResponseDateUtc });
    }
}
