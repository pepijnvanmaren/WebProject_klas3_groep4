using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebProject_klas3_groep4.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Gebruikers",
                columns: table => new
                {
                    ID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Naam = table.Column<string>(type: "TEXT", nullable: false),
                    Telefoonnummer = table.Column<int>(type: "INTEGER", nullable: false),
                    Email = table.Column<string>(type: "TEXT", nullable: false),
                    Rol = table.Column<string>(type: "TEXT", nullable: false),
                    Discriminator = table.Column<string>(type: "TEXT", maxLength: 21, nullable: false),
                    VeilingVestiging = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Gebruikers", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Veilingen",
                columns: table => new
                {
                    ID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    StarTijd = table.Column<string>(type: "TEXT", nullable: false),
                    StartDatum = table.Column<string>(type: "TEXT", nullable: false),
                    AantalProducten = table.Column<int>(type: "INTEGER", nullable: false),
                    KlokLocatie = table.Column<string>(type: "TEXT", nullable: false),
                    HuidigeSituatieVanVeiling = table.Column<string>(type: "TEXT", nullable: false),
                    Bechrijving = table.Column<string>(type: "TEXT", nullable: false),
                    VeilingmeesterDBID = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Veilingen", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Veilingen_Gebruikers_VeilingmeesterDBID",
                        column: x => x.VeilingmeesterDBID,
                        principalTable: "Gebruikers",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "LotDB",
                columns: table => new
                {
                    ID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ProductID = table.Column<int>(type: "INTEGER", nullable: false),
                    VeilingID = table.Column<int>(type: "INTEGER", nullable: false),
                    GebeurtenisDatum = table.Column<DateTime>(type: "TEXT", nullable: true),
                    GewensteLocatie = table.Column<string>(type: "TEXT", nullable: true),
                    VeilingDBID = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LotDB", x => x.ID);
                    table.ForeignKey(
                        name: "FK_LotDB_Veilingen_VeilingDBID",
                        column: x => x.VeilingDBID,
                        principalTable: "Veilingen",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateIndex(
                name: "IX_LotDB_VeilingDBID",
                table: "LotDB",
                column: "VeilingDBID");

            migrationBuilder.CreateIndex(
                name: "IX_Veilingen_VeilingmeesterDBID",
                table: "Veilingen",
                column: "VeilingmeesterDBID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LotDB");

            migrationBuilder.DropTable(
                name: "Veilingen");

            migrationBuilder.DropTable(
                name: "Gebruikers");
        }
    }
}
