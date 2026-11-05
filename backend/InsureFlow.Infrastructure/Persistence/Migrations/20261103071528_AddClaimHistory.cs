
using System;
using Microsoft.EntityFrameworkCore.Migrations;
using MySql.EntityFrameworkCore.Metadata;

#nullable disable

namespace InsureFlow.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddClaimHistory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "claim_history",
                columns: table => new
                {
                    id = table.Column<ulong>(
                        type: "bigint unsigned",
                        nullable: false)
                        .Annotation(
                            "MySQL:ValueGenerationStrategy",
                            MySQLValueGenerationStrategy.IdentityColumn),

                    tenant_id = table.Column<ulong>(
                        type: "bigint unsigned",
                        nullable: false),

                    claim_id = table.Column<ulong>(
                        type: "bigint unsigned",
                        nullable: false),

                    from_status = table.Column<string>(
                        type: "varchar(30)",
                        maxLength: 30,
                        nullable: false),

                    to_status = table.Column<string>(
                        type: "varchar(30)",
                        maxLength: 30,
                        nullable: false),

                    note = table.Column<string>(
                        type: "text",
                        nullable: true),

                    changed_at = table.Column<DateTime>(
                        type: "datetime(6)",
                        nullable: false),

                    changed_by_user_id = table.Column<ulong>(
                        type: "bigint unsigned",
                        nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey(
                        "PK_claim_history",
                        x => x.id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "ix_claim_history_claim_id_id",
                table: "claim_history",
                columns: new[]
                {
                    "claim_id",
                    "id"
                });

            migrationBuilder.CreateIndex(
                name: "ix_claim_history_tenant_claim_changed_at",
                table: "claim_history",
                columns: new[]
                {
                    "tenant_id",
                    "claim_id",
                    "changed_at"
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "claim_history");
        }
    }
}
