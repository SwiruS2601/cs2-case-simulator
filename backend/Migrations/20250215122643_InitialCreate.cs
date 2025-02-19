using Microsoft.EntityFrameworkCore.Migrations;

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
                name: "Cases",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: true),
                    Type = table.Column<string>(type: "TEXT", nullable: true),
                    FirstSaleDate = table.Column<string>(type: "TEXT", nullable: true),
                    MarketHashName = table.Column<string>(type: "TEXT", nullable: true),
                    Rental = table.Column<bool>(type: "INTEGER", nullable: true),
                    Image = table.Column<string>(type: "TEXT", nullable: true),
                    ModelPlayer = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cases", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Skins",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: true),
                    Classid = table.Column<string>(type: "TEXT", nullable: true),
                    Type = table.Column<string>(type: "TEXT", nullable: true),
                    WeaponType = table.Column<string>(type: "TEXT", nullable: true),
                    GunType = table.Column<string>(type: "TEXT", nullable: true),
                    Rarity = table.Column<string>(type: "TEXT", nullable: true),
                    RarityColor = table.Column<string>(type: "TEXT", nullable: true),
                    Prices = table.Column<string>(type: "TEXT", nullable: true),
                    FirstSaleDate = table.Column<string>(type: "TEXT", nullable: true),
                    KnifeType = table.Column<string>(type: "TEXT", nullable: true),
                    Image = table.Column<string>(type: "TEXT", nullable: true),
                    MinFloat = table.Column<double>(type: "REAL", nullable: true),
                    MaxFloat = table.Column<double>(type: "REAL", nullable: true),
                    Stattrak = table.Column<bool>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Skins", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CasesSkins",
                columns: table => new
                {
                    CasesId = table.Column<string>(type: "TEXT", nullable: false),
                    SkinsId = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CasesSkins", x => new { x.CasesId, x.SkinsId });
                    table.ForeignKey(
                        name: "FK_CasesSkins_Cases_CasesId",
                        column: x => x.CasesId,
                        principalTable: "Cases",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CasesSkins_Skins_SkinsId",
                        column: x => x.SkinsId,
                        principalTable: "Skins",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Cases_Name",
                table: "Cases",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_CasesSkins_SkinsId",
                table: "CasesSkins",
                column: "SkinsId");

            migrationBuilder.CreateIndex(
                name: "IX_Skins_Name",
                table: "Skins",
                column: "Name");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CasesSkins");

            migrationBuilder.DropTable(
                name: "Cases");

            migrationBuilder.DropTable(
                name: "Skins");
        }
    }
}
