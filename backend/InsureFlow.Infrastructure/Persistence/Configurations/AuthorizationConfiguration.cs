
using InsureFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InsureFlow.Infrastructure.Persistence.Configurations;

public sealed class AuthorizationConfiguration
    : IEntityTypeConfiguration<Authorization>
{
    public void Configure(EntityTypeBuilder<Authorization> b)
    {
        b.ToTable("Authorizations");

        b.HasKey(x => x.Id);

        b.Property(x => x.Id)
            .HasColumnName("id")
            .HasColumnType("bigint unsigned")
            .ValueGeneratedOnAdd();

        b.Property(x => x.ExternalId)
            .HasColumnName("authorization_guid")
            .HasColumnType("char(36)")
            .IsRequired();

        b.HasIndex(x => x.ExternalId)
            .IsUnique();

        b.Property(x => x.TenantId)
            .HasColumnName("tenant_id")
            .HasColumnType("bigint unsigned")
            .IsRequired();

        b.Property(x => x.PatientId)
            .HasColumnName("patient_id")
            .HasColumnType("bigint unsigned")
            .IsRequired();

        b.Property(x => x.PatientInsuranceId)
            .HasColumnName("patient_insurance_id")
            .HasColumnType("bigint unsigned")
            .IsRequired();

        b.Property(x => x.PayerId)
            .HasColumnName("payer_id")
            .HasColumnType("bigint unsigned")
            .IsRequired();

        b.Property(x => x.ProviderId)
            .HasColumnName("provider_id")
            .HasColumnType("bigint unsigned")
            .IsRequired();

        b.Property(x => x.AuthorizationNumber)
            .HasColumnName("authorization_number")
            .HasMaxLength(60)
            .IsRequired();

        b.Property(x => x.RequestType)
            .HasColumnName("request_type")
            .HasMaxLength(30)
            .IsRequired();

        b.Property(x => x.Status)
            .HasColumnName("status")
            .HasMaxLength(30)
            .IsRequired();

        b.Property(x => x.Priority)
            .HasColumnName("priority")
            .HasMaxLength(20)
            .IsRequired();

        b.Property(x => x.PlaceOfService)
            .HasColumnName("place_of_service")
            .HasMaxLength(10);

        b.Property(x => x.RequestedFrom)
            .HasColumnName("requested_from")
            .HasColumnType("date");

        b.Property(x => x.RequestedTo)
            .HasColumnName("requested_to")
            .HasColumnType("date");

        b.Property(x => x.DecisionDate)
            .HasColumnName("decision_date")
            .HasColumnType("date");

        b.Property(x => x.PayerReference)
            .HasColumnName("payer_reference")
            .HasMaxLength(100);

        b.Property(x => x.ClinicalSummary)
            .HasColumnName("clinical_summary")
            .HasColumnType("text");

        b.Property(x => x.DecisionReason)
            .HasColumnName("decision_reason")
            .HasMaxLength(1000);

        b.Property(x => x.CreatedAtUtc)
            .HasColumnName("created_at")
            .HasColumnType("datetime(6)")
            .IsRequired();

        b.Property(x => x.UpdatedAtUtc)
            .HasColumnName("updated_at")
            .HasColumnType("datetime(6)");
    }
}
