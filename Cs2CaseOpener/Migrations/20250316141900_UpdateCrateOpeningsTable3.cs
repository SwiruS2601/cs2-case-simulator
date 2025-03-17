using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cs2CaseOpener.Migrations
{
    /// <inheritdoc />
    public partial class UpdateCrateOpeningsTable3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "crate_name",
                table: "CrateOpenings",
                type: "character varying(120)",
                maxLength: 120,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "rarity",
                table: "CrateOpenings",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "skin_name",
                table: "CrateOpenings",
                type: "character varying(120)",
                maxLength: 120,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "wear_category",
                table: "CrateOpenings",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "crate_name",
                table: "CrateOpenings");

            migrationBuilder.DropColumn(
                name: "rarity",
                table: "CrateOpenings");

            migrationBuilder.DropColumn(
                name: "skin_name",
                table: "CrateOpenings");

            migrationBuilder.DropColumn(
                name: "wear_category",
                table: "CrateOpenings");
        }
    }
}
