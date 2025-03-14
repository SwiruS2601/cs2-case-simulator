using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Cs2CaseOpener.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Crates",
                columns: table => new
                {
                    id = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    name = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: true),
                    description = table.Column<string>(type: "varchar(1000)", maxLength: 1000, nullable: true),
                    type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    first_sale_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    market_hash_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    rental = table.Column<bool>(type: "boolean", nullable: true),
                    image = table.Column<string>(type: "character varying(400)", maxLength: 400, nullable: true),
                    model_player = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Crates", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Rarities",
                columns: table => new
                {
                    id = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    name = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: true),
                    color = table.Column<string>(type: "character varying(7)", maxLength: 7, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rarities", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Skins",
                columns: table => new
                {
                    id = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    name = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: true),
                    rarity_id = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    paint_index = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    image = table.Column<string>(type: "character varying(400)", maxLength: 400, nullable: true),
                    min_float = table.Column<double>(type: "double precision", nullable: true),
                    max_float = table.Column<double>(type: "double precision", nullable: true),
                    stat_trak = table.Column<bool>(type: "boolean", nullable: true),
                    souvenir = table.Column<bool>(type: "boolean", nullable: true),
                    category = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    pattern = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Skins", x => x.id);
                    table.ForeignKey(
                        name: "FK_Skins_Rarities_rarity_id",
                        column: x => x.rarity_id,
                        principalTable: "Rarities",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "CrateSkins",
                columns: table => new
                {
                    CratesId = table.Column<string>(type: "character varying(50)", nullable: false),
                    SkinsId = table.Column<string>(type: "character varying(50)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CrateSkins", x => new { x.CratesId, x.SkinsId });
                    table.ForeignKey(
                        name: "FK_CrateSkins_Crates_CratesId",
                        column: x => x.CratesId,
                        principalTable: "Crates",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CrateSkins_Skins_SkinsId",
                        column: x => x.SkinsId,
                        principalTable: "Skins",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Prices",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    skin_id = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    crate_id = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    name = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    wear_category = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    steam_last_24h = table.Column<double>(type: "double precision", nullable: true),
                    steam_last_7d = table.Column<double>(type: "double precision", nullable: true),
                    steam_last_30d = table.Column<double>(type: "double precision", nullable: true),
                    steam_last_90d = table.Column<double>(type: "double precision", nullable: true),
                    steam_last_ever = table.Column<double>(type: "double precision", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Prices", x => x.id);
                    table.ForeignKey(
                        name: "FK_Prices_Crates_crate_id",
                        column: x => x.crate_id,
                        principalTable: "Crates",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_Prices_Skins_skin_id",
                        column: x => x.skin_id,
                        principalTable: "Skins",
                        principalColumn: "id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Crates_name",
                table: "Crates",
                column: "name");

            migrationBuilder.CreateIndex(
                name: "IX_CrateSkins_SkinsId",
                table: "CrateSkins",
                column: "SkinsId");

            migrationBuilder.CreateIndex(
                name: "IX_Prices_crate_id",
                table: "Prices",
                column: "crate_id",
                unique: true,
                filter: "crate_id IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Prices_name",
                table: "Prices",
                column: "name");

            migrationBuilder.CreateIndex(
                name: "IX_Prices_skin_id_wear_category",
                table: "Prices",
                columns: new[] { "skin_id", "wear_category" },
                unique: true,
                filter: "skin_id IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Rarities_name",
                table: "Rarities",
                column: "name");

            migrationBuilder.CreateIndex(
                name: "IX_Skins_name",
                table: "Skins",
                column: "name");

            migrationBuilder.CreateIndex(
                name: "IX_Skins_rarity_id",
                table: "Skins",
                column: "rarity_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CrateSkins");

            migrationBuilder.DropTable(
                name: "Prices");

            migrationBuilder.DropTable(
                name: "Crates");

            migrationBuilder.DropTable(
                name: "Skins");

            migrationBuilder.DropTable(
                name: "Rarities");
        }
    }
}
