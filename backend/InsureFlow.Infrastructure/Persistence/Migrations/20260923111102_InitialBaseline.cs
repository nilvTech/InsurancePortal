using System;
using Microsoft.EntityFrameworkCore.Migrations;
using MySql.EntityFrameworkCore.Metadata;

#nullable disable

namespace InsureFlow.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialBaseline : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "tenants",
                columns: table => new
                {
                    id = table.Column<ulong>(type: "bigint unsigned", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    tenant_name = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false),
                    tenant_code = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false),
                    legal_name = table.Column<string>(type: "varchar(250)", maxLength: 250, nullable: true),
                    tax_identifier = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true),
                    status = table.Column<string>(type: "enum('TRIAL','ACTIVE','SUSPENDED','INACTIVE')", nullable: false),
                    subscription_plan = table.Column<string>(type: "enum('STARTER','PROFESSIONAL','ENTERPRISE')", nullable: false),
                    timezone = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false),
                    contact_first_name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true),
                    contact_last_name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true),
                    contact_email = table.Column<string>(type: "varchar(254)", maxLength: 254, nullable: true),
                    contact_phone = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: true),
                    tenant_uuid = table.Column<Guid>(type: "char(36)", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tenants", x => x.id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "organizations",
                columns: table => new
                {
                    id = table.Column<ulong>(type: "bigint unsigned", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    tenant_id = table.Column<ulong>(type: "bigint unsigned", nullable: false),
                    organization_code = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false),
                    organization_name = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false),
                    legal_name = table.Column<string>(type: "varchar(250)", maxLength: 250, nullable: true),
                    organization_type = table.Column<string>(type: "enum('MEDICAL_GROUP','HOSPITAL','HEALTH_SYSTEM','PRACTICE','OTHER')", nullable: false),
                    tax_id = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true),
                    npi = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: true),
                    phone = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: true),
                    fax = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: true),
                    email = table.Column<string>(type: "varchar(254)", maxLength: 254, nullable: true),
                    website = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true),
                    status = table.Column<string>(type: "enum('ACTIVE','INACTIVE')", nullable: false),
                    organization_uuid = table.Column<Guid>(type: "char(36)", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_organizations", x => x.id);
                    table.ForeignKey(
                        name: "fk_organizations_tenant_id_1",
                        column: x => x.tenant_id,
                        principalTable: "tenants",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "patients",
                columns: table => new
                {
                    id = table.Column<ulong>(type: "bigint unsigned", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    tenant_id = table.Column<ulong>(type: "bigint unsigned", nullable: false),
                    patient_id = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false),
                    mrn = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false),
                    account_number = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false),
                    first_name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false),
                    middle_name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true),
                    last_name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false),
                    preferred_name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true),
                    date_of_birth = table.Column<DateTime>(type: "date", nullable: false),
                    gender = table.Column<string>(type: "enum('UNKNOWN','MALE','FEMALE','NON_BINARY','OTHER')", nullable: false),
                    patient_status = table.Column<string>(type: "enum('ACTIVE','INACTIVE','DECEASED','MERGED')", nullable: false),
                    registration_date = table.Column<DateTime>(type: "date", nullable: true),
                    last_visit_date = table.Column<DateTime>(type: "date", nullable: true),
                    patient_class = table.Column<string>(type: "enum('OUTPATIENT','INPATIENT','OBSERVATION','EMERGENCY','OTHER')", nullable: true),
                    portal_access = table.Column<string>(type: "enum('DISABLED','ENABLED','PENDING')", nullable: false),
                    communication_consent = table.Column<string>(type: "enum('PENDING','GRANTED','DECLINED')", nullable: false),
                    preferred_pharmacy = table.Column<string>(type: "varchar(250)", maxLength: 250, nullable: true),
                    referral_source = table.Column<string>(type: "enum('PHYSICIAN','HOSPITAL','SELF','INSURANCE','EMPLOYER','OTHER')", nullable: true),
                    financial_assistance = table.Column<string>(type: "enum('NONE','PENDING','APPROVED','DECLINED')", nullable: false),
                    notes = table.Column<string>(type: "text", nullable: true),
                    patient_uuid = table.Column<Guid>(type: "char(36)", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_patients", x => x.id);
                    table.ForeignKey(
                        name: "fk_patients_tenant_id_1",
                        column: x => x.tenant_id,
                        principalTable: "tenants",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "payers",
                columns: table => new
                {
                    id = table.Column<ulong>(type: "bigint unsigned", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    tenant_id = table.Column<ulong>(type: "bigint unsigned", nullable: false),
                    payer_code = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false),
                    payer_name = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false),
                    payer_id = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true),
                    payer_type = table.Column<string>(type: "enum('COMMERCIAL','MEDICARE','MEDICAID','TRICARE','WORKERS_COMP','OTHER')", nullable: false),
                    phone = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: true),
                    website = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true),
                    status = table.Column<string>(type: "enum('ACTIVE','INACTIVE')", nullable: false),
                    payer_uuid = table.Column<Guid>(type: "char(36)", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_payers", x => x.id);
                    table.ForeignKey(
                        name: "fk_payers_tenant_id_1",
                        column: x => x.tenant_id,
                        principalTable: "tenants",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "providers",
                columns: table => new
                {
                    id = table.Column<ulong>(type: "bigint unsigned", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    tenant_id = table.Column<ulong>(type: "bigint unsigned", nullable: false),
                    organization_id = table.Column<ulong>(type: "bigint unsigned", nullable: false),
                    provider_code = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false),
                    first_name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false),
                    middle_name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true),
                    last_name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false),
                    credential = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true),
                    npi = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: false),
                    specialty = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: true),
                    department = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: true),
                    phone = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: true),
                    email = table.Column<string>(type: "varchar(254)", maxLength: 254, nullable: true),
                    status = table.Column<string>(type: "enum('ACTIVE','INACTIVE')", nullable: false),
                    provider_uuid = table.Column<Guid>(type: "char(36)", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_providers", x => x.id);
                    table.ForeignKey(
                        name: "fk_providers_organization_id_2",
                        column: x => x.organization_id,
                        principalTable: "organizations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_providers_tenant_id_1",
                        column: x => x.tenant_id,
                        principalTable: "tenants",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "idx_organizations_organization_name",
                table: "organizations",
                column: "organization_name");

            migrationBuilder.CreateIndex(
                name: "idx_organizations_status",
                table: "organizations",
                column: "status");

            migrationBuilder.CreateIndex(
                name: "idx_organizations_tenant_id",
                table: "organizations",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "uk_organizations_1",
                table: "organizations",
                columns: new[] { "tenant_id", "organization_code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "uk_organizations_2",
                table: "organizations",
                columns: new[] { "tenant_id", "npi" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "uk_organizations_organization_uuid",
                table: "organizations",
                column: "organization_uuid",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "idx_patients_date_of_birth",
                table: "patients",
                column: "date_of_birth");

            migrationBuilder.CreateIndex(
                name: "idx_patients_first_name",
                table: "patients",
                column: "first_name");

            migrationBuilder.CreateIndex(
                name: "idx_patients_last_name",
                table: "patients",
                column: "last_name");

            migrationBuilder.CreateIndex(
                name: "idx_patients_patient_class",
                table: "patients",
                column: "patient_class");

            migrationBuilder.CreateIndex(
                name: "idx_patients_patient_status",
                table: "patients",
                column: "patient_status");

            migrationBuilder.CreateIndex(
                name: "idx_patients_tenant_id",
                table: "patients",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "uk_patients_1",
                table: "patients",
                columns: new[] { "tenant_id", "patient_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "uk_patients_2",
                table: "patients",
                columns: new[] { "tenant_id", "mrn" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "uk_patients_3",
                table: "patients",
                columns: new[] { "tenant_id", "account_number" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "uk_patients_patient_uuid",
                table: "patients",
                column: "patient_uuid",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "idx_payers_payer_id",
                table: "payers",
                column: "payer_id");

            migrationBuilder.CreateIndex(
                name: "idx_payers_payer_name",
                table: "payers",
                column: "payer_name");

            migrationBuilder.CreateIndex(
                name: "idx_payers_status",
                table: "payers",
                column: "status");

            migrationBuilder.CreateIndex(
                name: "idx_payers_tenant_id",
                table: "payers",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "uk_payers_1",
                table: "payers",
                columns: new[] { "tenant_id", "payer_code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "uk_payers_payer_uuid",
                table: "payers",
                column: "payer_uuid",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "idx_providers_last_name",
                table: "providers",
                column: "last_name");

            migrationBuilder.CreateIndex(
                name: "idx_providers_organization_id",
                table: "providers",
                column: "organization_id");

            migrationBuilder.CreateIndex(
                name: "idx_providers_specialty",
                table: "providers",
                column: "specialty");

            migrationBuilder.CreateIndex(
                name: "idx_providers_status",
                table: "providers",
                column: "status");

            migrationBuilder.CreateIndex(
                name: "idx_providers_tenant_id",
                table: "providers",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "uk_providers_1",
                table: "providers",
                columns: new[] { "tenant_id", "provider_code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "uk_providers_2",
                table: "providers",
                columns: new[] { "tenant_id", "npi" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "uk_providers_provider_uuid",
                table: "providers",
                column: "provider_uuid",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "idx_tenants_status",
                table: "tenants",
                column: "status");

            migrationBuilder.CreateIndex(
                name: "idx_tenants_tenant_name",
                table: "tenants",
                column: "tenant_name");

            migrationBuilder.CreateIndex(
                name: "uk_tenants_tax_identifier",
                table: "tenants",
                column: "tax_identifier",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "uk_tenants_tenant_code",
                table: "tenants",
                column: "tenant_code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "uk_tenants_tenant_uuid",
                table: "tenants",
                column: "tenant_uuid",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "patients");

            migrationBuilder.DropTable(
                name: "payers");

            migrationBuilder.DropTable(
                name: "providers");

            migrationBuilder.DropTable(
                name: "organizations");

            migrationBuilder.DropTable(
                name: "tenants");
        }
    }
}
