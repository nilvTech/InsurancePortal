
using System;
using InsureFlow.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InsureFlow.Infrastructure.Persistence.Migrations;

[DbContext(typeof(InsureFlowDbContext))]
[Migration("20261022100000_AlignClaimsSchema")]
public partial class AlignClaimsSchema : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        // Add the missing claim submission and tracking columns.
        migrationBuilder.AddColumn<string>(
            name: "submission_reference",
            table: "claims",
            type: "varchar(100)",
            maxLength: 100,
            nullable: true);

        migrationBuilder.AddColumn<DateTime>(
            name: "submitted_at",
            table: "claims",
            type: "datetime(6)",
            nullable: true);

        migrationBuilder.AddColumn<DateTime>(
            name: "last_status_checked_at",
            table: "claims",
            type: "datetime(6)",
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "notes",
            table: "claims",
            type: "text",
            nullable: true);

        // Add claim-line fields missing from the existing database.
        migrationBuilder.AddColumn<string>(
            name: "claim_line_uuid",
            table: "claim_lines",
            type: "char(36)",
            maxLength: 36,
            nullable: true);

        migrationBuilder.AddColumn<DateTime>(
            name: "service_date",
            table: "claim_lines",
            type: "date",
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "modifier3",
            table: "claim_lines",
            type: "varchar(10)",
            maxLength: 10,
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "modifier4",
            table: "claim_lines",
            type: "varchar(10)",
            maxLength: 10,
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "place_of_service",
            table: "claim_lines",
            type: "varchar(10)",
            maxLength: 10,
            nullable: true);

        // Preserve existing records by backfilling the new columns.
        migrationBuilder.Sql(
            """
            UPDATE claim_lines
            SET claim_line_uuid = UUID()
            WHERE claim_line_uuid IS NULL;
            """);

        migrationBuilder.Sql(
            """
            UPDATE claim_lines cl
            INNER JOIN claims c ON c.id = cl.claim_id
            SET cl.service_date = c.service_from
            WHERE cl.service_date IS NULL;
            """);

        migrationBuilder.AlterColumn<string>(
            name: "claim_line_uuid",
            table: "claim_lines",
            type: "char(36)",
            maxLength: 36,
            nullable: false,
            oldClrType: typeof(string),
            oldType: "char(36)",
            oldMaxLength: 36,
            oldNullable: true);

        migrationBuilder.AlterColumn<DateTime>(
            name: "service_date",
            table: "claim_lines",
            type: "date",
            nullable: false,
            oldClrType: typeof(DateTime),
            oldType: "date",
            oldNullable: true);

        // Permit multiple lines per claim, while preventing duplicate
        // line numbers within the same claim.
        migrationBuilder.DropIndex(
            name: "uk_claim_lines_claim_id",
            table: "claim_lines");

        migrationBuilder.DropIndex(
            name: "uk_claim_lines_line_number",
            table: "claim_lines");

        migrationBuilder.CreateIndex(
            name: "uk_claim_lines_claim_line_number",
            table: "claim_lines",
            columns: new[] { "claim_id", "line_number" },
            unique: true);

        migrationBuilder.CreateIndex(
            name: "uk_claim_lines_claim_line_uuid",
            table: "claim_lines",
            column: "claim_line_uuid",
            unique: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropIndex(
            name: "uk_claim_lines_claim_line_number",
            table: "claim_lines");

        migrationBuilder.DropIndex(
            name: "uk_claim_lines_claim_line_uuid",
            table: "claim_lines");

        migrationBuilder.CreateIndex(
            name: "uk_claim_lines_claim_id",
            table: "claim_lines",
            column: "claim_id",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "uk_claim_lines_line_number",
            table: "claim_lines",
            column: "line_number",
            unique: true);

        migrationBuilder.DropColumn(
            name: "claim_line_uuid",
            table: "claim_lines");

        migrationBuilder.DropColumn(
            name: "service_date",
            table: "claim_lines");

        migrationBuilder.DropColumn(
            name: "modifier3",
            table: "claim_lines");

        migrationBuilder.DropColumn(
            name: "modifier4",
            table: "claim_lines");

        migrationBuilder.DropColumn(
            name: "place_of_service",
            table: "claim_lines");

        migrationBuilder.DropColumn(
            name: "submission_reference",
            table: "claims");

        migrationBuilder.DropColumn(
            name: "submitted_at",
            table: "claims");

        migrationBuilder.DropColumn(
            name: "last_status_checked_at",
            table: "claims");

        migrationBuilder.DropColumn(
            name: "notes",
            table: "claims");
    }
}
