
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
            // Create only the two missing Denials tables.
            // Existing Denials, Appeals and Claims tables remain untouched.

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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "denial_activities");

            migrationBuilder.DropTable(
                name: "denial_reasons");
        }
    }
}
