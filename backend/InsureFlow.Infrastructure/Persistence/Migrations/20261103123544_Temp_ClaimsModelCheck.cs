using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InsureFlow.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Temp_ClaimsModelCheck : Migration
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

            migrationBuilder.AddColumn<ulong>(
                name: "TempId",
                table: "claims",
                type: "bigint unsigned",
                nullable: false,
                defaultValue: 0ul);

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
