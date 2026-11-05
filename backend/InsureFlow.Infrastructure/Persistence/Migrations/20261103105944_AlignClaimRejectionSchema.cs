using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InsureFlow.Infrastructure.Persistence.Migrations
{
    public partial class AlignClaimRejectionSchema : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // The claim_rejections table already exists in the database.
            // Its columns, indexes, and foreign key already match the
            // corrected ClaimRejection entity configuration.
            //
            // No database schema changes are required.
            // This migration only registers the corrected EF Core model.
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Intentionally empty.
            // The existing claim_rejections table and its data must be preserved.
        }
    }
}