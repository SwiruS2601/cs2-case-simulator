using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Cs2CaseOpener.Migrations
{
    /// <inheritdoc />
    public partial class AddCrateOpeningsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CrateOpenings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    crate_id = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    skin_id = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    client_id = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    user_agent = table.Column<string>(type: "character varying(400)", maxLength: 400, nullable: true),
                    timezone = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    user_id = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    session_id = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    client_ip = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    opened_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CrateOpenings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CrateOpenings_Crates_crate_id",
                        column: x => x.crate_id,
                        principalTable: "Crates",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CrateOpenings_Skins_skin_id",
                        column: x => x.skin_id,
                        principalTable: "Skins",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CrateOpenings_client_id",
                table: "CrateOpenings",
                column: "client_id");

            migrationBuilder.CreateIndex(
                name: "IX_CrateOpenings_crate_id",
                table: "CrateOpenings",
                column: "crate_id");

            migrationBuilder.CreateIndex(
                name: "IX_CrateOpenings_opened_at",
                table: "CrateOpenings",
                column: "opened_at");

            migrationBuilder.CreateIndex(
                name: "IX_CrateOpenings_skin_id",
                table: "CrateOpenings",
                column: "skin_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CrateOpenings");
        }
    }
}
