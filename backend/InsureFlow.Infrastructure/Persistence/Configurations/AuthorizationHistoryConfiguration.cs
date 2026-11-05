using InsureFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InsureFlow.Infrastructure.Persistence.Configurations;

public sealed class AuthorizationHistoryConfiguration
    : IEntityTypeConfiguration<AuthorizationHistory>
{
    public void Configure(EntityTypeBuilder<AuthorizationHistory> b)
    {
        b.ToTable("authorization_history");

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

        b.Property(x => x.FromStatus)
            .HasColumnName("from_status")
            .HasMaxLength(30)
            .IsRequired();

        b.Property(x => x.ToStatus)
            .HasColumnName("to_status")
            .HasMaxLength(30)
            .IsRequired();

        b.Property(x => x.Note)
            .HasColumnName("note")
            .HasColumnType("text");

        b.Property(x => x.ChangedAt)
            .HasColumnName("changed_at")
            .HasColumnType("datetime(6)")
            .IsRequired();

        b.Property(x => x.ChangedByUserId)
            .HasColumnName("changed_by_user_id")
            .HasColumnType("bigint unsigned");

        b.HasIndex(x => new
        {
            x.TenantId,
            x.AuthorizationId,
            x.ChangedAt
        });
    }
}