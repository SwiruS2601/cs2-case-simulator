using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cs2CaseOpener.Migrations
{
    /// <inheritdoc />
    public partial class UpdateCrateOpeningsTable2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "timezone",
                table: "CrateOpenings");

            migrationBuilder.DropColumn(
                name: "user_agent",
                table: "CrateOpenings");

            migrationBuilder.DropColumn(
                name: "user_id",
                table: "CrateOpenings");

            migrationBuilder.AlterColumn<string>(
                name: "skin_id",
                table: "CrateOpenings",
                type: "character varying(25)",
                maxLength: 25,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "crate_id",
                table: "CrateOpenings",
                type: "character varying(25)",
                maxLength: 25,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "skin_id",
                table: "CrateOpenings",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(25)",
                oldMaxLength: 25);

            migrationBuilder.AlterColumn<string>(
                name: "crate_id",
                table: "CrateOpenings",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(25)",
                oldMaxLength: 25);

            migrationBuilder.AddColumn<string>(
                name: "timezone",
                table: "CrateOpenings",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "user_agent",
                table: "CrateOpenings",
                type: "character varying(400)",
                maxLength: 400,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "user_id",
                table: "CrateOpenings",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);
        }
    }
}
