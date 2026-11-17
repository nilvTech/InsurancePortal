using System;
using Microsoft.EntityFrameworkCore.Migrations;
using MySql.EntityFrameworkCore.Metadata;

#nullable disable

namespace InsureFlow.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddPaymentsModule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "eob_records",
                columns: table => new
                {
                    id = table.Column<ulong>(type: "bigint unsigned", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    tenant_id = table.Column<ulong>(type: "bigint unsigned", nullable: false),
                    payer_id = table.Column<ulong>(type: "bigint unsigned", nullable: false),
                    patient_id = table.Column<ulong>(type: "bigint unsigned", nullable: true),
                    claim_id = table.Column<ulong>(type: "bigint unsigned", nullable: true),
                    document_number = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true),
                    file_name = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true),
                    content_type = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: true),
                    storage_reference = table.Column<string>(type: "varchar(1000)", maxLength: 1000, nullable: true),
                    file_size_bytes = table.Column<long>(type: "bigint", nullable: true),
                    received_at_utc = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    statement_date_utc = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    status = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: false),
                    notes = table.Column<string>(type: "varchar(2000)", maxLength: 2000, nullable: true),
                    eob_guid = table.Column<Guid>(type: "char(36)", nullable: false),
                    created_at_utc = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    updated_at_utc = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    CreatedByUserId = table.Column<long>(type: "bigint", nullable: true),
                    UpdatedByUserId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_eob_records", x => x.id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "era_records",
                columns: table => new
                {
                    id = table.Column<ulong>(type: "bigint unsigned", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    tenant_id = table.Column<ulong>(type: "bigint unsigned", nullable: false),
                    payer_id = table.Column<ulong>(type: "bigint unsigned", nullable: false),
                    trace_number = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true),
                    check_number = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true),
                    total_payment_amount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    payment_date_utc = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    received_at_utc = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    file_name = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true),
                    storage_reference = table.Column<string>(type: "varchar(1000)", maxLength: 1000, nullable: true),
                    transaction_control_number = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true),
                    import_status = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: false),
                    error_message = table.Column<string>(type: "varchar(4000)", maxLength: 4000, nullable: true),
                    raw_content = table.Column<string>(type: "longtext", nullable: true),
                    era_guid = table.Column<Guid>(type: "char(36)", nullable: false),
                    created_at_utc = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    updated_at_utc = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    CreatedByUserId = table.Column<long>(type: "bigint", nullable: true),
                    UpdatedByUserId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_era_records", x => x.id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "payment_adjustments",
                columns: table => new
                {
                    id = table.Column<ulong>(type: "bigint unsigned", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    tenant_id = table.Column<ulong>(type: "bigint unsigned", nullable: false),
                    payment_id = table.Column<ulong>(type: "bigint unsigned", nullable: false),
                    payment_allocation_id = table.Column<ulong>(type: "bigint unsigned", nullable: true),
                    claim_id = table.Column<ulong>(type: "bigint unsigned", nullable: true),
                    adjustment_code = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: false),
                    adjustment_group_code = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: true),
                    description = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: false),
                    adjustment_amount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    adjustment_date_utc = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    status = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: false),
                    notes = table.Column<string>(type: "varchar(2000)", maxLength: 2000, nullable: true),
                    adjustment_guid = table.Column<Guid>(type: "char(36)", nullable: false),
                    created_at_utc = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    updated_at_utc = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    CreatedByUserId = table.Column<long>(type: "bigint", nullable: true),
                    UpdatedByUserId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_payment_adjustments", x => x.id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "payment_reconciliations",
                columns: table => new
                {
                    id = table.Column<ulong>(type: "bigint unsigned", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    tenant_id = table.Column<ulong>(type: "bigint unsigned", nullable: false),
                    reconciliation_number = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false),
                    period_start_utc = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    period_end_utc = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    started_at_utc = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    completed_at_utc = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    expected_amount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    actual_amount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    difference_amount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    payments_processed = table.Column<int>(type: "int", nullable: false),
                    payments_matched = table.Column<int>(type: "int", nullable: false),
                    payments_unmatched = table.Column<int>(type: "int", nullable: false),
                    status = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: false),
                    summary = table.Column<string>(type: "varchar(4000)", maxLength: 4000, nullable: true),
                    error_message = table.Column<string>(type: "varchar(4000)", maxLength: 4000, nullable: true),
                    reconciliation_guid = table.Column<Guid>(type: "char(36)", nullable: false),
                    created_at_utc = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    updated_at_utc = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    CreatedByUserId = table.Column<long>(type: "bigint", nullable: true),
                    UpdatedByUserId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_payment_reconciliations", x => x.id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "payments",
                columns: table => new
                {
                    id = table.Column<ulong>(type: "bigint unsigned", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    tenant_id = table.Column<ulong>(type: "bigint unsigned", nullable: false),
                    payment_number = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false),
                    payer_id = table.Column<ulong>(type: "bigint unsigned", nullable: false),
                    patient_id = table.Column<ulong>(type: "bigint unsigned", nullable: true),
                    patient_insurance_id = table.Column<ulong>(type: "bigint unsigned", nullable: true),
                    check_number = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true),
                    trace_number = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true),
                    payment_method = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: false),
                    payment_amount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    allocated_amount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    unallocated_amount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    currency = table.Column<string>(type: "varchar(3)", maxLength: 3, nullable: false),
                    payment_date_utc = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    received_date_utc = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    posted_at_utc = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    reversed_at_utc = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    status = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: false),
                    notes = table.Column<string>(type: "varchar(2000)", maxLength: 2000, nullable: true),
                    payment_guid = table.Column<Guid>(type: "char(36)", nullable: false),
                    created_at_utc = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    updated_at_utc = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    CreatedByUserId = table.Column<long>(type: "bigint", nullable: true),
                    UpdatedByUserId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_payments", x => x.id);
                    table.ForeignKey(
                        name: "FK_payments_payers_payer_id",
                        column: x => x.payer_id,
                        principalTable: "payers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "payment_allocations",
                columns: table => new
                {
                    id = table.Column<ulong>(type: "bigint unsigned", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    tenant_id = table.Column<ulong>(type: "bigint unsigned", nullable: false),
                    payment_id = table.Column<ulong>(type: "bigint unsigned", nullable: false),
                    claim_id = table.Column<ulong>(type: "bigint unsigned", nullable: false),
                    claim_line_id = table.Column<ulong>(type: "bigint unsigned", nullable: true),
                    allocated_amount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    allocation_date_utc = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    status = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: false),
                    notes = table.Column<string>(type: "varchar(2000)", maxLength: 2000, nullable: true),
                    allocation_guid = table.Column<Guid>(type: "char(36)", nullable: false),
                    created_at_utc = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    updated_at_utc = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    CreatedByUserId = table.Column<long>(type: "bigint", nullable: true),
                    UpdatedByUserId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_payment_allocations", x => x.id);
                    table.ForeignKey(
                        name: "FK_payment_allocations_claims_claim_id",
                        column: x => x.claim_id,
                        principalTable: "claims",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_payment_allocations_payments_payment_id",
                        column: x => x.payment_id,
                        principalTable: "payments",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "refunds",
                columns: table => new
                {
                    id = table.Column<ulong>(type: "bigint unsigned", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    tenant_id = table.Column<ulong>(type: "bigint unsigned", nullable: false),
                    payment_id = table.Column<ulong>(type: "bigint unsigned", nullable: false),
                    refund_number = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false),
                    refund_amount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    reason = table.Column<string>(type: "varchar(1000)", maxLength: 1000, nullable: false),
                    status = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: false),
                    requested_at_utc = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    requested_by_user_id = table.Column<ulong>(type: "bigint unsigned", nullable: true),
                    reviewed_at_utc = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    reviewed_by_user_id = table.Column<ulong>(type: "bigint unsigned", nullable: true),
                    review_notes = table.Column<string>(type: "varchar(2000)", maxLength: 2000, nullable: true),
                    processed_at_utc = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    refund_guid = table.Column<Guid>(type: "char(36)", nullable: false),
                    created_at_utc = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    updated_at_utc = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    CreatedByUserId = table.Column<long>(type: "bigint", nullable: true),
                    UpdatedByUserId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_refunds", x => x.id);
                    table.ForeignKey(
                        name: "FK_refunds_payments_payment_id",
                        column: x => x.payment_id,
                        principalTable: "payments",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_eob_records_eob_guid",
                table: "eob_records",
                column: "eob_guid",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_eob_records_tenant_id_claim_id",
                table: "eob_records",
                columns: new[] { "tenant_id", "claim_id" });

            migrationBuilder.CreateIndex(
                name: "IX_eob_records_tenant_id_payer_id_received_at_utc",
                table: "eob_records",
                columns: new[] { "tenant_id", "payer_id", "received_at_utc" });

            migrationBuilder.CreateIndex(
                name: "IX_era_records_era_guid",
                table: "era_records",
                column: "era_guid",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_era_records_tenant_id_payer_id_received_at_utc",
                table: "era_records",
                columns: new[] { "tenant_id", "payer_id", "received_at_utc" });

            migrationBuilder.CreateIndex(
                name: "IX_era_records_tenant_id_trace_number",
                table: "era_records",
                columns: new[] { "tenant_id", "trace_number" });

            migrationBuilder.CreateIndex(
                name: "IX_payment_adjustments_adjustment_guid",
                table: "payment_adjustments",
                column: "adjustment_guid",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_payment_adjustments_tenant_id_claim_id",
                table: "payment_adjustments",
                columns: new[] { "tenant_id", "claim_id" });

            migrationBuilder.CreateIndex(
                name: "IX_payment_adjustments_tenant_id_payment_id",
                table: "payment_adjustments",
                columns: new[] { "tenant_id", "payment_id" });

            migrationBuilder.CreateIndex(
                name: "IX_payment_allocations_allocation_guid",
                table: "payment_allocations",
                column: "allocation_guid",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_payment_allocations_claim_id",
                table: "payment_allocations",
                column: "claim_id");

            migrationBuilder.CreateIndex(
                name: "IX_payment_allocations_payment_id",
                table: "payment_allocations",
                column: "payment_id");

            migrationBuilder.CreateIndex(
                name: "IX_payment_allocations_tenant_id_claim_id",
                table: "payment_allocations",
                columns: new[] { "tenant_id", "claim_id" });

            migrationBuilder.CreateIndex(
                name: "IX_payment_allocations_tenant_id_payment_id",
                table: "payment_allocations",
                columns: new[] { "tenant_id", "payment_id" });

            migrationBuilder.CreateIndex(
                name: "IX_payment_reconciliations_reconciliation_guid",
                table: "payment_reconciliations",
                column: "reconciliation_guid",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_payment_reconciliations_tenant_id_reconciliation_number",
                table: "payment_reconciliations",
                columns: new[] { "tenant_id", "reconciliation_number" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_payment_reconciliations_tenant_id_started_at_utc",
                table: "payment_reconciliations",
                columns: new[] { "tenant_id", "started_at_utc" });

            migrationBuilder.CreateIndex(
                name: "IX_payments_payer_id",
                table: "payments",
                column: "payer_id");

            migrationBuilder.CreateIndex(
                name: "IX_payments_payment_guid",
                table: "payments",
                column: "payment_guid",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_payments_tenant_id_payer_id_payment_date_utc",
                table: "payments",
                columns: new[] { "tenant_id", "payer_id", "payment_date_utc" });

            migrationBuilder.CreateIndex(
                name: "IX_payments_tenant_id_payment_number",
                table: "payments",
                columns: new[] { "tenant_id", "payment_number" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_payments_tenant_id_status",
                table: "payments",
                columns: new[] { "tenant_id", "status" });

            migrationBuilder.CreateIndex(
                name: "IX_refunds_payment_id",
                table: "refunds",
                column: "payment_id");

            migrationBuilder.CreateIndex(
                name: "IX_refunds_refund_guid",
                table: "refunds",
                column: "refund_guid",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_refunds_tenant_id_payment_id",
                table: "refunds",
                columns: new[] { "tenant_id", "payment_id" });

            migrationBuilder.CreateIndex(
                name: "IX_refunds_tenant_id_refund_number",
                table: "refunds",
                columns: new[] { "tenant_id", "refund_number" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_refunds_tenant_id_status",
                table: "refunds",
                columns: new[] { "tenant_id", "status" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "eob_records");

            migrationBuilder.DropTable(
                name: "era_records");

            migrationBuilder.DropTable(
                name: "payment_adjustments");

            migrationBuilder.DropTable(
                name: "payment_allocations");

            migrationBuilder.DropTable(
                name: "payment_reconciliations");

            migrationBuilder.DropTable(
                name: "refunds");

            migrationBuilder.DropTable(
                name: "payments");
        }
    }
}
