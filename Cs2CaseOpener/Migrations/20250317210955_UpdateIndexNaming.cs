using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cs2CaseOpener.Migrations
{
    /// <inheritdoc />
    public partial class UpdateIndexNaming : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_CrateOpenings_OpenedAt_Rarity",
                table: "CrateOpenings",
                columns: new[] { "opened_at", "rarity" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_CrateOpenings_OpenedAt_Rarity",
                table: "CrateOpenings");
        }
    }
}
