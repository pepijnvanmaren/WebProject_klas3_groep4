using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebProject_klas3_groep4.Migrations
{
    /// <inheritdoc />
    public partial class dbmaken : Migration
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
                    Paswoord = table.Column<string>(type: "TEXT", nullable: false),
                    Rol = table.Column<string>(type: "TEXT", nullable: false),
                    Discriminator = table.Column<string>(type: "TEXT", maxLength: 21, nullable: false),
                    VeilingVestiging = table.Column<string>(type: "TEXT", nullable: true),
                    KvkNummer = table.Column<string>(type: "TEXT", nullable: true),
                    NaamVanBedrijf = table.Column<string>(type: "TEXT", nullable: true),
                    Postcode = table.Column<string>(type: "TEXT", nullable: true),
                    Adres = table.Column<string>(type: "TEXT", nullable: true),
                    BedrijfTelefoonnummer = table.Column<string>(type: "TEXT", nullable: true),
                    BedrijfEmail = table.Column<string>(type: "TEXT", nullable: true),
                    BankGegevens = table.Column<string>(type: "TEXT", nullable: true),
                    KoperDB_Postcode = table.Column<string>(type: "TEXT", nullable: true),
                    KoperDB_Adres = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Gebruikers", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "product",
                columns: table => new
                {
                    ID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Naam = table.Column<string>(type: "TEXT", nullable: true),
                    Foto = table.Column<string>(type: "TEXT", nullable: true),
                    Beschrijving = table.Column<string>(type: "TEXT", nullable: true),
                    AanvoerderDBID = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_product", x => x.ID);
                    table.ForeignKey(
                        name: "FK_product_Gebruikers_AanvoerderDBID",
                        column: x => x.AanvoerderDBID,
                        principalTable: "Gebruikers",
                        principalColumn: "ID");
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
                name: "Lot",
                columns: table => new
                {
                    ID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ProductID = table.Column<int>(type: "INTEGER", nullable: false),
                    VeilingID = table.Column<int>(type: "INTEGER", nullable: false),
                    GebeurtenisDatum = table.Column<DateTime>(type: "TEXT", nullable: true),
                    GewensteLocatie = table.Column<string>(type: "TEXT", nullable: true),
                    KoperDBID = table.Column<int>(type: "INTEGER", nullable: true),
                    VeilingDBID = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Lot", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Lot_Gebruikers_KoperDBID",
                        column: x => x.KoperDBID,
                        principalTable: "Gebruikers",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_Lot_Veilingen_VeilingDBID",
                        column: x => x.VeilingDBID,
                        principalTable: "Veilingen",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_Lot_product_ProductID",
                        column: x => x.ProductID,
                        principalTable: "product",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Gebruikers",
                columns: new[] { "ID", "Discriminator", "Email", "Naam", "Rol", "Telefoonnummer" },
                values: new object[] { 1, "GebruikerDB", "admin@example.com", "Admin", "Administrator", 123456789 });

            migrationBuilder.CreateIndex(
                name: "IX_Lot_KoperDBID",
                table: "Lot",
                column: "KoperDBID");

            migrationBuilder.CreateIndex(
                name: "IX_Lot_ProductID",
                table: "Lot",
                column: "ProductID");

            migrationBuilder.CreateIndex(
                name: "IX_Lot_VeilingDBID",
                table: "Lot",
                column: "VeilingDBID");

            migrationBuilder.CreateIndex(
                name: "IX_product_AanvoerderDBID",
                table: "product",
                column: "AanvoerderDBID");

            migrationBuilder.CreateIndex(
                name: "IX_Veilingen_VeilingmeesterDBID",
                table: "Veilingen",
                column: "VeilingmeesterDBID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Lot");

            migrationBuilder.DropTable(
                name: "Veilingen");

            migrationBuilder.DropTable(
                name: "product");

            migrationBuilder.DropTable(
                name: "Gebruikers");
        }
    }
}
