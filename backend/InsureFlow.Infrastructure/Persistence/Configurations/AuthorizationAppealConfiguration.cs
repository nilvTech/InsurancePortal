using InsureFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InsureFlow.Infrastructure.Persistence.Configurations;

public sealed class AuthorizationAppealConfiguration
    : IEntityTypeConfiguration<AuthorizationAppeal>
{
    public void Configure(
        EntityTypeBuilder<AuthorizationAppeal> b)
    {
        b.ToTable("authorization_appeals");

        b.HasKey(x => x.Id);

        b.Property(x => x.Id)
            .HasColumnName("id")
            .HasColumnType("bigint unsigned")
            .ValueGeneratedOnAdd();

        b.Property(x => x.TenantId)
            .HasColumnName("tenant_id")
            .HasColumnType("bigint unsigned")
            .IsRequired();

        b.Property(x => x.AuthorizationId)
            .HasColumnName("authorization_id")
            .HasColumnType("bigint unsigned")
            .IsRequired();

        b.Property(x => x.Reason)
            .HasColumnName("reason")
            .HasMaxLength(1000)
            .IsRequired();

        b.Property(x => x.Notes)
            .HasColumnName("notes")
            .HasColumnType("text");

        b.Property(x => x.Status)
            .HasColumnName("status")
            .HasMaxLength(30)
            .IsRequired();

        b.Property(x => x.SubmittedAt)
            .HasColumnName("submitted_at")
            .HasColumnType("datetime(6)")
            .IsRequired();

        b.HasIndex(x => new
        {
            x.TenantId,
            x.AuthorizationId,
            x.SubmittedAt
        });
    }
}