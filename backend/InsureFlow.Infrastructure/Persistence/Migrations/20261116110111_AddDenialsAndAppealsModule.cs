using System;
using Microsoft.EntityFrameworkCore.Migrations;
using MySql.EntityFrameworkCore.Metadata;

#nullable disable

namespace InsureFlow.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddDenialsAndAppealsModule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_claim_rejections_claims_claim_id",
                table: "claim_rejections");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_claims_TempId",
                table: "claims");

            migrationBuilder.DropColumn(
                name: "TempId",
                table: "claims");

            migrationBuilder.AlterColumn<ulong>(
                name: "id",
                table: "claims",
                type: "bigint unsigned",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint")
                .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn)
                .OldAnnotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<ulong>(
                name: "claim_id",
                table: "claim_lines",
                type: "bigint unsigned",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.CreateTable(
                name: "claim_attachments",
                columns: table => new
                {
                    id = table.Column<ulong>(type: "bigint unsigned", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    tenant_id = table.Column<ulong>(type: "bigint unsigned", nullable: false),
                    claim_id = table.Column<ulong>(type: "bigint unsigned", nullable: false),
                    file_name = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false),
                    storage_key = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: false),
                    content_type = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true),
                    file_size = table.Column<ulong>(type: "bigint unsigned", nullable: true),
                    document_type = table.Column<string>(type: "varchar(40)", maxLength: 40, nullable: false),
                    uploaded_at = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_claim_attachments", x => x.id);
                    table.ForeignKey(
                        name: "FK_claim_attachments_claims_claim_id",
                        column: x => x.claim_id,
                        principalTable: "claims",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_claim_attachments_tenants_tenant_id",
                        column: x => x.tenant_id,
                        principalTable: "tenants",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "claim_diagnoses",
                columns: table => new
                {
                    id = table.Column<ulong>(type: "bigint unsigned", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    tenant_id = table.Column<ulong>(type: "bigint unsigned", nullable: false),
                    claim_id = table.Column<ulong>(type: "bigint unsigned", nullable: false),
                    icd10_code = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false),
                    description = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true),
                    sequence = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_claim_diagnoses", x => x.id);
                    table.ForeignKey(
                        name: "FK_claim_diagnoses_claims_claim_id",
                        column: x => x.claim_id,
                        principalTable: "claims",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_claim_diagnoses_tenants_tenant_id",
                        column: x => x.tenant_id,
                        principalTable: "tenants",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "denial_reasons",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    tenant_id = table.Column<ulong>(type: "bigint unsigned", nullable: false),
                    reason_code = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false),
                    reason_name = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false),
                    description = table.Column<string>(type: "text", nullable: true),
                    category = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false),
                    is_active = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    reason_guid = table.Column<Guid>(type: "char(36)", nullable: false),
                    created_at_utc = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    updated_at_utc = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    CreatedByUserId = table.Column<long>(type: "bigint", nullable: true),
                    UpdatedByUserId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_denial_reasons", x => x.Id);
                    table.ForeignKey(
                        name: "FK_denial_reasons_tenants_tenant_id",
                        column: x => x.tenant_id,
                        principalTable: "tenants",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "denials",
                columns: table => new
                {
                    id = table.Column<ulong>(type: "bigint unsigned", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    tenant_id = table.Column<ulong>(type: "bigint unsigned", nullable: false),
                    claim_id = table.Column<ulong>(type: "bigint unsigned", nullable: false),
                    denial_number = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false),
                    denial_code = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false),
                    denial_reason = table.Column<string>(type: "varchar(1000)", maxLength: 1000, nullable: false),
                    denial_category = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false),
                    denied_amount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    denial_date = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    appeal_deadline = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    status = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: false),
                    assigned_to_user_id = table.Column<long>(type: "bigint", nullable: true),
                    denial_guid = table.Column<Guid>(type: "char(36)", nullable: false),
                    created_at_utc = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    updated_at_utc = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_denials", x => x.id);
                    table.ForeignKey(
                        name: "FK_denials_claims_claim_id",
                        column: x => x.claim_id,
                        principalTable: "claims",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_denials_tenants_tenant_id",
                        column: x => x.tenant_id,
                        principalTable: "tenants",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "appeals",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    tenant_id = table.Column<long>(type: "bigint", nullable: false),
                    denial_id = table.Column<ulong>(type: "bigint unsigned", nullable: false),
                    appeal_number = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false),
                    appeal_level = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false),
                    appeal_reason = table.Column<string>(type: "varchar(2000)", maxLength: 2000, nullable: false),
                    appeal_notes = table.Column<string>(type: "varchar(4000)", maxLength: 4000, nullable: true),
                    status = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: false),
                    appeal_deadline_utc = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    submitted_at_utc = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    withdrawn_at_utc = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    decision = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: true),
                    decision_notes = table.Column<string>(type: "varchar(4000)", maxLength: 4000, nullable: true),
                    decision_at_utc = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    external_id = table.Column<Guid>(type: "char(36)", nullable: false),
                    created_at_utc = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    updated_at_utc = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    created_by_user_id = table.Column<long>(type: "bigint", nullable: true),
                    updated_by_user_id = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_appeals", x => x.id);
                    table.ForeignKey(
                        name: "FK_appeals_denials_denial_id",
                        column: x => x.denial_id,
                        principalTable: "denials",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "denial_activities",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    tenant_id = table.Column<ulong>(type: "bigint unsigned", nullable: false),
                    denial_id = table.Column<ulong>(type: "bigint unsigned", nullable: false),
                    activity_type = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false),
                    description = table.Column<string>(type: "varchar(2000)", maxLength: 2000, nullable: false),
                    performed_by_user_id = table.Column<long>(type: "bigint", nullable: true),
                    activity_date_utc = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    activity_guid = table.Column<Guid>(type: "char(36)", nullable: false),
                    created_at_utc = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    updated_at_utc = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_denial_activities", x => x.id);
                    table.ForeignKey(
                        name: "FK_denial_activities_denials_denial_id",
                        column: x => x.denial_id,
                        principalTable: "denials",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_denial_activities_tenants_tenant_id",
                        column: x => x.tenant_id,
                        principalTable: "tenants",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "denial_attachments",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    tenant_id = table.Column<ulong>(type: "bigint unsigned", nullable: false),
                    denial_id = table.Column<ulong>(type: "bigint unsigned", nullable: false),
                    file_name = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false),
                    content_type = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: false),
                    file_size_bytes = table.Column<long>(type: "bigint", nullable: false),
                    storage_reference = table.Column<string>(type: "varchar(1000)", maxLength: 1000, nullable: false),
                    uploaded_by_user_id = table.Column<long>(type: "bigint", nullable: true),
                    uploaded_at_utc = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    attachment_guid = table.Column<Guid>(type: "char(36)", nullable: false),
                    created_at_utc = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    updated_at_utc = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_denial_attachments", x => x.id);
                    table.ForeignKey(
                        name: "FK_denial_attachments_denials_denial_id",
                        column: x => x.denial_id,
                        principalTable: "denials",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_denial_attachments_tenants_tenant_id",
                        column: x => x.tenant_id,
                        principalTable: "tenants",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "denial_payer_responses",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    tenant_id = table.Column<ulong>(type: "bigint unsigned", nullable: false),
                    denial_id = table.Column<ulong>(type: "bigint unsigned", nullable: false),
                    response_type = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false),
                    response_code = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true),
                    response_message = table.Column<string>(type: "text", nullable: false),
                    payer_reference_number = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: true),
                    response_date_utc = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    received_by_user_id = table.Column<long>(type: "bigint", nullable: true),
                    response_guid = table.Column<Guid>(type: "char(36)", nullable: false),
                    created_at_utc = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    updated_at_utc = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    CreatedByUserId = table.Column<long>(type: "bigint", nullable: true),
                    UpdatedByUserId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_denial_payer_responses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_denial_payer_responses_denials_denial_id",
                        column: x => x.denial_id,
                        principalTable: "denials",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_denial_payer_responses_tenants_tenant_id",
                        column: x => x.tenant_id,
                        principalTable: "tenants",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "denial_resolutions",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    tenant_id = table.Column<ulong>(type: "bigint unsigned", nullable: false),
                    denial_id = table.Column<ulong>(type: "bigint unsigned", nullable: false),
                    resolution_type = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false),
                    resolution_notes = table.Column<string>(type: "text", nullable: false),
                    resolved_amount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    resolution_date_utc = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    resolved_by_user_id = table.Column<long>(type: "bigint", nullable: true),
                    resolution_guid = table.Column<Guid>(type: "char(36)", nullable: false),
                    created_at_utc = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    updated_at_utc = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    CreatedByUserId = table.Column<long>(type: "bigint", nullable: true),
                    UpdatedByUserId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_denial_resolutions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_denial_resolutions_denials_denial_id",
                        column: x => x.denial_id,
                        principalTable: "denials",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_denial_resolutions_tenants_tenant_id",
                        column: x => x.tenant_id,
                        principalTable: "tenants",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "appeal_attachments",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    tenant_id = table.Column<long>(type: "bigint", nullable: false),
                    appeal_id = table.Column<long>(type: "bigint", nullable: false),
                    file_name = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false),
                    content_type = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: false),
                    file_size_bytes = table.Column<long>(type: "bigint", nullable: false),
                    storage_reference = table.Column<string>(type: "varchar(1000)", maxLength: 1000, nullable: false),
                    uploaded_at_utc = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    uploaded_by_user_id = table.Column<long>(type: "bigint", nullable: true),
                    external_id = table.Column<Guid>(type: "char(36)", nullable: false),
                    created_at_utc = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    updated_at_utc = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    created_by_user_id = table.Column<long>(type: "bigint", nullable: true),
                    updated_by_user_id = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_appeal_attachments", x => x.id);
                    table.ForeignKey(
                        name: "FK_appeal_attachments_appeals_appeal_id",
                        column: x => x.appeal_id,
                        principalTable: "appeals",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "appeal_documentation",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    tenant_id = table.Column<long>(type: "bigint", nullable: false),
                    appeal_id = table.Column<long>(type: "bigint", nullable: false),
                    document_title = table.Column<string>(type: "varchar(250)", maxLength: 250, nullable: false),
                    document_type = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false),
                    description = table.Column<string>(type: "varchar(2000)", maxLength: 2000, nullable: true),
                    storage_reference = table.Column<string>(type: "varchar(1000)", maxLength: 1000, nullable: true),
                    external_id = table.Column<Guid>(type: "char(36)", nullable: false),
                    created_at_utc = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    updated_at_utc = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    created_by_user_id = table.Column<long>(type: "bigint", nullable: true),
                    updated_by_user_id = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_appeal_documentation", x => x.id);
                    table.ForeignKey(
                        name: "FK_appeal_documentation_appeals_appeal_id",
                        column: x => x.appeal_id,
                        principalTable: "appeals",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "appeal_external_reviews",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    tenant_id = table.Column<long>(type: "bigint", nullable: false),
                    appeal_id = table.Column<long>(type: "bigint", nullable: false),
                    reviewing_organization = table.Column<string>(type: "varchar(250)", maxLength: 250, nullable: false),
                    external_reference_number = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true),
                    status = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: false),
                    requested_at_utc = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    review_due_at_utc = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    responded_at_utc = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    review_notes = table.Column<string>(type: "varchar(4000)", maxLength: 4000, nullable: true),
                    external_id = table.Column<Guid>(type: "char(36)", nullable: false),
                    created_at_utc = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    updated_at_utc = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    created_by_user_id = table.Column<long>(type: "bigint", nullable: true),
                    updated_by_user_id = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_appeal_external_reviews", x => x.id);
                    table.ForeignKey(
                        name: "FK_appeal_external_reviews_appeals_appeal_id",
                        column: x => x.appeal_id,
                        principalTable: "appeals",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "appeal_history",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    tenant_id = table.Column<long>(type: "bigint", nullable: false),
                    appeal_id = table.Column<long>(type: "bigint", nullable: false),
                    activity_type = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false),
                    description = table.Column<string>(type: "varchar(2000)", maxLength: 2000, nullable: false),
                    performed_by_user_id = table.Column<long>(type: "bigint", nullable: true),
                    activity_date_utc = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    external_id = table.Column<Guid>(type: "char(36)", nullable: false),
                    created_at_utc = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    updated_at_utc = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    created_by_user_id = table.Column<long>(type: "bigint", nullable: true),
                    updated_by_user_id = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_appeal_history", x => x.id);
                    table.ForeignKey(
                        name: "FK_appeal_history_appeals_appeal_id",
                        column: x => x.appeal_id,
                        principalTable: "appeals",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "appeal_notifications",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    tenant_id = table.Column<long>(type: "bigint", nullable: false),
                    appeal_id = table.Column<long>(type: "bigint", nullable: false),
                    notification_type = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false),
                    title = table.Column<string>(type: "varchar(250)", maxLength: 250, nullable: false),
                    message = table.Column<string>(type: "varchar(2000)", maxLength: 2000, nullable: false),
                    recipient_user_id = table.Column<long>(type: "bigint", nullable: true),
                    is_read = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    read_at_utc = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    external_id = table.Column<Guid>(type: "char(36)", nullable: false),
                    created_at_utc = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    updated_at_utc = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    created_by_user_id = table.Column<long>(type: "bigint", nullable: true),
                    updated_by_user_id = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_appeal_notifications", x => x.id);
                    table.ForeignKey(
                        name: "FK_appeal_notifications_appeals_appeal_id",
                        column: x => x.appeal_id,
                        principalTable: "appeals",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_appeal_attachments_appeal_id",
                table: "appeal_attachments",
                column: "appeal_id");

            migrationBuilder.CreateIndex(
                name: "IX_appeal_attachments_external_id",
                table: "appeal_attachments",
                column: "external_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_appeal_attachments_tenant_id_appeal_id",
                table: "appeal_attachments",
                columns: new[] { "tenant_id", "appeal_id" });

            migrationBuilder.CreateIndex(
                name: "IX_appeal_documentation_appeal_id",
                table: "appeal_documentation",
                column: "appeal_id");

            migrationBuilder.CreateIndex(
                name: "IX_appeal_documentation_external_id",
                table: "appeal_documentation",
                column: "external_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_appeal_documentation_tenant_id_appeal_id",
                table: "appeal_documentation",
                columns: new[] { "tenant_id", "appeal_id" });

            migrationBuilder.CreateIndex(
                name: "IX_appeal_external_reviews_appeal_id",
                table: "appeal_external_reviews",
                column: "appeal_id");

            migrationBuilder.CreateIndex(
                name: "IX_appeal_external_reviews_external_id",
                table: "appeal_external_reviews",
                column: "external_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_appeal_external_reviews_tenant_id_appeal_id",
                table: "appeal_external_reviews",
                columns: new[] { "tenant_id", "appeal_id" });

            migrationBuilder.CreateIndex(
                name: "IX_appeal_external_reviews_tenant_id_status",
                table: "appeal_external_reviews",
                columns: new[] { "tenant_id", "status" });

            migrationBuilder.CreateIndex(
                name: "IX_appeal_history_appeal_id",
                table: "appeal_history",
                column: "appeal_id");

            migrationBuilder.CreateIndex(
                name: "IX_appeal_history_external_id",
                table: "appeal_history",
                column: "external_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_appeal_history_tenant_id_appeal_id_activity_date_utc",
                table: "appeal_history",
                columns: new[] { "tenant_id", "appeal_id", "activity_date_utc" });

            migrationBuilder.CreateIndex(
                name: "IX_appeal_notifications_appeal_id",
                table: "appeal_notifications",
                column: "appeal_id");

            migrationBuilder.CreateIndex(
                name: "IX_appeal_notifications_external_id",
                table: "appeal_notifications",
                column: "external_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_appeal_notifications_tenant_id_appeal_id_is_read",
                table: "appeal_notifications",
                columns: new[] { "tenant_id", "appeal_id", "is_read" });

            migrationBuilder.CreateIndex(
                name: "IX_appeal_notifications_tenant_id_recipient_user_id",
                table: "appeal_notifications",
                columns: new[] { "tenant_id", "recipient_user_id" });

            migrationBuilder.CreateIndex(
                name: "IX_appeals_denial_id",
                table: "appeals",
                column: "denial_id");

            migrationBuilder.CreateIndex(
                name: "IX_appeals_external_id",
                table: "appeals",
                column: "external_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_appeals_tenant_id_appeal_number",
                table: "appeals",
                columns: new[] { "tenant_id", "appeal_number" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_appeals_tenant_id_denial_id",
                table: "appeals",
                columns: new[] { "tenant_id", "denial_id" });

            migrationBuilder.CreateIndex(
                name: "IX_appeals_tenant_id_status",
                table: "appeals",
                columns: new[] { "tenant_id", "status" });

            migrationBuilder.CreateIndex(
                name: "IX_claim_attachments_claim_id",
                table: "claim_attachments",
                column: "claim_id");

            migrationBuilder.CreateIndex(
                name: "IX_claim_attachments_tenant_id_claim_id",
                table: "claim_attachments",
                columns: new[] { "tenant_id", "claim_id" });

            migrationBuilder.CreateIndex(
                name: "IX_claim_diagnoses_claim_id",
                table: "claim_diagnoses",
                column: "claim_id");

            migrationBuilder.CreateIndex(
                name: "IX_claim_diagnoses_tenant_id_claim_id_sequence",
                table: "claim_diagnoses",
                columns: new[] { "tenant_id", "claim_id", "sequence" });

            migrationBuilder.CreateIndex(
                name: "IX_denial_activities_activity_guid",
                table: "denial_activities",
                column: "activity_guid",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_denial_activities_denial_id",
                table: "denial_activities",
                column: "denial_id");

            migrationBuilder.CreateIndex(
                name: "IX_denial_activities_tenant_id_denial_id_activity_date_utc",
                table: "denial_activities",
                columns: new[] { "tenant_id", "denial_id", "activity_date_utc" });

            migrationBuilder.CreateIndex(
                name: "IX_denial_attachments_attachment_guid",
                table: "denial_attachments",
                column: "attachment_guid",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_denial_attachments_denial_id",
                table: "denial_attachments",
                column: "denial_id");

            migrationBuilder.CreateIndex(
                name: "IX_denial_attachments_tenant_id_denial_id",
                table: "denial_attachments",
                columns: new[] { "tenant_id", "denial_id" });

            migrationBuilder.CreateIndex(
                name: "IX_denial_payer_responses_denial_id_response_date_utc",
                table: "denial_payer_responses",
                columns: new[] { "denial_id", "response_date_utc" });

            migrationBuilder.CreateIndex(
                name: "IX_denial_payer_responses_response_guid",
                table: "denial_payer_responses",
                column: "response_guid",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_denial_payer_responses_tenant_id_denial_id",
                table: "denial_payer_responses",
                columns: new[] { "tenant_id", "denial_id" });

            migrationBuilder.CreateIndex(
                name: "IX_denial_reasons_reason_guid",
                table: "denial_reasons",
                column: "reason_guid",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_denial_reasons_tenant_id_reason_code",
                table: "denial_reasons",
                columns: new[] { "tenant_id", "reason_code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_denial_resolutions_denial_id_resolution_date_utc",
                table: "denial_resolutions",
                columns: new[] { "denial_id", "resolution_date_utc" });

            migrationBuilder.CreateIndex(
                name: "IX_denial_resolutions_resolution_guid",
                table: "denial_resolutions",
                column: "resolution_guid",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_denial_resolutions_tenant_id_denial_id",
                table: "denial_resolutions",
                columns: new[] { "tenant_id", "denial_id" });

            migrationBuilder.CreateIndex(
                name: "IX_denials_claim_id",
                table: "denials",
                column: "claim_id");

            migrationBuilder.CreateIndex(
                name: "IX_denials_denial_guid",
                table: "denials",
                column: "denial_guid",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_denials_tenant_id_claim_id",
                table: "denials",
                columns: new[] { "tenant_id", "claim_id" });

            migrationBuilder.CreateIndex(
                name: "IX_denials_tenant_id_denial_date",
                table: "denials",
                columns: new[] { "tenant_id", "denial_date" });

            migrationBuilder.CreateIndex(
                name: "IX_denials_tenant_id_denial_number",
                table: "denials",
                columns: new[] { "tenant_id", "denial_number" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_denials_tenant_id_status",
                table: "denials",
                columns: new[] { "tenant_id", "status" });

            migrationBuilder.AddForeignKey(
                name: "FK_claim_rejections_claims_claim_id",
                table: "claim_rejections",
                column: "claim_id",
                principalTable: "claims",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_claim_rejections_claims_claim_id",
                table: "claim_rejections");

            migrationBuilder.DropTable(
                name: "appeal_attachments");

            migrationBuilder.DropTable(
                name: "appeal_documentation");

            migrationBuilder.DropTable(
                name: "appeal_external_reviews");

            migrationBuilder.DropTable(
                name: "appeal_history");

            migrationBuilder.DropTable(
                name: "appeal_notifications");

            migrationBuilder.DropTable(
                name: "claim_attachments");

            migrationBuilder.DropTable(
                name: "claim_diagnoses");

            migrationBuilder.DropTable(
                name: "denial_activities");

            migrationBuilder.DropTable(
                name: "denial_attachments");

            migrationBuilder.DropTable(
                name: "denial_payer_responses");

            migrationBuilder.DropTable(
                name: "denial_reasons");

            migrationBuilder.DropTable(
                name: "denial_resolutions");

            migrationBuilder.DropTable(
                name: "appeals");

            migrationBuilder.DropTable(
                name: "denials");

            migrationBuilder.AlterColumn<long>(
                name: "id",
                table: "claims",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(ulong),
                oldType: "bigint unsigned")
                .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn)
                .OldAnnotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AddColumn<ulong>(
                name: "TempId",
                table: "claims",
                type: "bigint unsigned",
                nullable: false,
                defaultValue: 0ul);

            migrationBuilder.AlterColumn<long>(
                name: "claim_id",
                table: "claim_lines",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(ulong),
                oldType: "bigint unsigned");

            migrationBuilder.AddUniqueConstraint(
                name: "AK_claims_TempId",
                table: "claims",
                column: "TempId");

            migrationBuilder.AddForeignKey(
                name: "FK_claim_rejections_claims_claim_id",
                table: "claim_rejections",
                column: "claim_id",
                principalTable: "claims",
                principalColumn: "TempId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
