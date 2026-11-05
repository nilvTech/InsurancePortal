using InsureFlow.Domain.Entities;
using InsureFlow.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InsureFlow.Infrastructure.Persistence.Configurations;

public sealed class PatientConfiguration
    : IEntityTypeConfiguration<Patient>
{
    public void Configure(EntityTypeBuilder<Patient> builder)
    {
        builder.ToTable("patients");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnName("id")
            .HasColumnType("bigint unsigned")
            .ValueGeneratedOnAdd();

        builder.Property(x => x.ExternalId)
            .HasColumnName("patient_uuid")
            .HasColumnType("char(36)")
            .IsRequired();

        builder.HasIndex(x => x.ExternalId)
            .IsUnique()
            .HasDatabaseName("uk_patients_patient_uuid");

        builder.Property(x => x.TenantId)
            .HasColumnName("tenant_id")
            .HasColumnType("bigint unsigned")
            .IsRequired();

        builder.HasIndex(x => x.TenantId)
            .HasDatabaseName("idx_patients_tenant_id");

        builder.Property(x => x.PatientId)
            .HasColumnName("patient_id")
            .HasMaxLength(50)
            .IsRequired();

        builder.HasIndex(x => new { x.TenantId, x.PatientId })
            .IsUnique()
            .HasDatabaseName("uk_patients_1");

        builder.Property(x => x.Mrn)
            .HasColumnName("mrn")
            .HasMaxLength(100)
            .IsRequired();

        builder.HasIndex(x => new { x.TenantId, x.Mrn })
            .IsUnique()
            .HasDatabaseName("uk_patients_2");

        builder.Property(x => x.AccountNumber)
            .HasColumnName("account_number")
            .HasMaxLength(100)
            .IsRequired();

        builder.HasIndex(x => new { x.TenantId, x.AccountNumber })
            .IsUnique()
            .HasDatabaseName("uk_patients_3");

        builder.Property(x => x.FirstName)
            .HasColumnName("first_name")
            .HasMaxLength(100)
            .IsRequired();

        builder.HasIndex(x => x.FirstName)
            .HasDatabaseName("idx_patients_first_name");

        builder.Property(x => x.MiddleName)
            .HasColumnName("middle_name")
            .HasMaxLength(100);

        builder.Property(x => x.LastName)
            .HasColumnName("last_name")
            .HasMaxLength(100)
            .IsRequired();

        builder.HasIndex(x => x.LastName)
            .HasDatabaseName("idx_patients_last_name");

        builder.Property(x => x.PreferredName)
            .HasColumnName("preferred_name")
            .HasMaxLength(100);

        builder.Property(x => x.DateOfBirth)
            .HasColumnName("date_of_birth")
            .HasColumnType("date")
            .IsRequired();

        builder.HasIndex(x => x.DateOfBirth)
            .HasDatabaseName("idx_patients_date_of_birth");

        builder.Property(x => x.Gender)
            .HasColumnName("gender")
            .HasColumnType(
                "enum('UNKNOWN','MALE','FEMALE','NON_BINARY','OTHER')")
            .HasConversion(
                value => value == PatientGender.Unknown
                    ? "UNKNOWN"
                    : value == PatientGender.Male
                        ? "MALE"
                        : value == PatientGender.Female
                            ? "FEMALE"
                            : value == PatientGender.NonBinary
                                ? "NON_BINARY"
                                : "OTHER",
                value => value == "UNKNOWN"
                    ? PatientGender.Unknown
                    : value == "MALE"
                        ? PatientGender.Male
                        : value == "FEMALE"
                            ? PatientGender.Female
                            : value == "NON_BINARY"
                                ? PatientGender.NonBinary
                                : PatientGender.Other)
            .IsRequired();

        builder.Property(x => x.Status)
            .HasColumnName("patient_status")
            .HasColumnType(
                "enum('ACTIVE','INACTIVE','DECEASED','MERGED')")
            .HasConversion(
                value => value == PatientStatus.Active
                    ? "ACTIVE"
                    : value == PatientStatus.Inactive
                        ? "INACTIVE"
                        : value == PatientStatus.Deceased
                            ? "DECEASED"
                            : "MERGED",
                value => value == "ACTIVE"
                    ? PatientStatus.Active
                    : value == "INACTIVE"
                        ? PatientStatus.Inactive
                        : value == "DECEASED"
                            ? PatientStatus.Deceased
                            : PatientStatus.Merged)
            .IsRequired();

        builder.HasIndex(x => x.Status)
            .HasDatabaseName("idx_patients_patient_status");

        builder.Property(x => x.RegistrationDate)
            .HasColumnName("registration_date")
            .HasColumnType("date");

        builder.Property(x => x.LastVisitDate)
            .HasColumnName("last_visit_date")
            .HasColumnType("date");

        builder.Property(x => x.PatientClass)
    .HasColumnName("patient_class")
    .HasColumnType(
        "enum('OUTPATIENT','INPATIENT','OBSERVATION','EMERGENCY','OTHER')")
    .HasConversion(
        value => value.HasValue
            ? value.Value == PatientClass.Outpatient
                ? "OUTPATIENT"
                : value.Value == PatientClass.Inpatient
                    ? "INPATIENT"
                    : value.Value == PatientClass.Observation
                        ? "OBSERVATION"
                        : value.Value == PatientClass.Emergency
                            ? "EMERGENCY"
                            : "OTHER"
            : null,
        value => value == null
            ? null
            : value == "OUTPATIENT"
                ? PatientClass.Outpatient
                : value == "INPATIENT"
                    ? PatientClass.Inpatient
                    : value == "OBSERVATION"
                        ? PatientClass.Observation
                        : value == "EMERGENCY"
                            ? PatientClass.Emergency
                            : PatientClass.Other)
    .IsRequired(false);

        builder.HasIndex(x => x.PatientClass)
            .HasDatabaseName("idx_patients_patient_class");

        builder.Property(x => x.PortalAccess)
            .HasColumnName("portal_access")
            .HasColumnType(
                "enum('DISABLED','ENABLED','PENDING')")
            .HasConversion(
                value => value == PortalAccessStatus.Disabled
                    ? "DISABLED"
                    : value == PortalAccessStatus.Enabled
                        ? "ENABLED"
                        : "PENDING",
                value => value == "DISABLED"
                    ? PortalAccessStatus.Disabled
                    : value == "ENABLED"
                        ? PortalAccessStatus.Enabled
                        : PortalAccessStatus.Pending)
            .IsRequired();

        builder.Property(x => x.CommunicationConsent)
            .HasColumnName("communication_consent")
            .HasColumnType(
                "enum('PENDING','GRANTED','DECLINED')")
            .HasConversion(
                value => value == CommunicationConsentStatus.Pending
                    ? "PENDING"
                    : value == CommunicationConsentStatus.Granted
                        ? "GRANTED"
                        : "DECLINED",
                value => value == "PENDING"
                    ? CommunicationConsentStatus.Pending
                    : value == "GRANTED"
                        ? CommunicationConsentStatus.Granted
                        : CommunicationConsentStatus.Declined)
            .IsRequired();

        builder.Property(x => x.PreferredPharmacy)
            .HasColumnName("preferred_pharmacy")
            .HasMaxLength(250);

        builder.Property(x => x.ReferralSource)
            .HasColumnName("referral_source")
            .HasColumnType(
                "enum('PHYSICIAN','HOSPITAL','SELF','INSURANCE','EMPLOYER','OTHER')")
            .HasConversion(
                value => value == ReferralSource.Physician
                    ? "PHYSICIAN"
                    : value == ReferralSource.Hospital
                        ? "HOSPITAL"
                        : value == ReferralSource.Self
                            ? "SELF"
                            : value == ReferralSource.Insurance
                                ? "INSURANCE"
                                : value == ReferralSource.Employer
                                    ? "EMPLOYER"
                                    : "OTHER",
                value => value == "PHYSICIAN"
                    ? ReferralSource.Physician
                    : value == "HOSPITAL"
                        ? ReferralSource.Hospital
                        : value == "SELF"
                            ? ReferralSource.Self
                            : value == "INSURANCE"
                                ? ReferralSource.Insurance
                                : value == "EMPLOYER"
                                    ? ReferralSource.Employer
                                    : ReferralSource.Other);

        builder.Property(x => x.FinancialAssistance)
            .HasColumnName("financial_assistance")
            .HasColumnType(
                "enum('NONE','PENDING','APPROVED','DECLINED')")
            .HasConversion(
                value => value == FinancialAssistanceStatus.None
                    ? "NONE"
                    : value == FinancialAssistanceStatus.Pending
                        ? "PENDING"
                        : value == FinancialAssistanceStatus.Approved
                            ? "APPROVED"
                            : "DECLINED",
                value => value == "NONE"
                    ? FinancialAssistanceStatus.None
                    : value == "PENDING"
                        ? FinancialAssistanceStatus.Pending
                        : value == "APPROVED"
                            ? FinancialAssistanceStatus.Approved
                            : FinancialAssistanceStatus.Declined)
            .IsRequired();

        builder.Property(x => x.Notes)
            .HasColumnName("notes")
            .HasColumnType("text");

        builder.Property(x => x.CreatedAtUtc)
            .HasColumnName("created_at")
            .HasColumnType("datetime(6)")
            .IsRequired();

        builder.Property(x => x.UpdatedAtUtc)
            .HasColumnName("updated_at")
            .HasColumnType("datetime(6)")
            .IsRequired();

        builder.Ignore(x => x.CreatedByUserId);
        builder.Ignore(x => x.UpdatedByUserId);

        builder.HasOne(x => x.Tenant)
            .WithMany()
            .HasForeignKey(x => x.TenantId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("fk_patients_tenant_id_1");
    }
}