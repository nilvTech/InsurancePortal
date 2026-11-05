using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InsureFlow.Infrastructure.Persistence.Migrations
{
    public partial class DiagnoseClaimRejectionModelDiff : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Intentionally empty.
            //
            // The current database already uses claims.id as the
            // principal key for claim_rejections.
            //
            // The old EF model snapshot incorrectly contained
            // claims.TempId as an alternate key.
            //
            // The generated operations would remove claims.TempId
            // and modify the foreign key. They must not be executed.
            //
            // The updated model snapshot is retained to synchronize
            // EF Core with the current application model.
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Intentionally empty.
            // This migration does not modify the database schema.
        }
    }
}