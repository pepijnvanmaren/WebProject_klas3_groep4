using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebProject_klas3_groep4.Migrations
{
    /// <inheritdoc />
    public partial class Initial_MSSQL : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Gebruikers",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Naam = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Telefoonnummer = table.Column<int>(type: "int", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Rol = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Paswoord = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Discriminator = table.Column<string>(type: "nvarchar(21)", maxLength: 21, nullable: false),
                    VeilingVestiging = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    KvkNummer = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NaamVanBedrijf = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Postcode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Adres = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BedrijfTelefoonnummer = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BedrijfEmail = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BankGegevens = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    KoperDB_Postcode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    KoperDB_Adres = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Gebruikers", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "product",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Naam = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Foto = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Beschrijving = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AanvoerderDBID = table.Column<int>(type: "int", nullable: true)
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
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StarTijd = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StartDatum = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AantalProducten = table.Column<int>(type: "int", nullable: false),
                    KlokLocatie = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    HuidigeSituatieVanVeiling = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Bechrijving = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    VeilingmeesterDBID = table.Column<int>(type: "int", nullable: true)
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
                name: "Lots",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductID = table.Column<int>(type: "int", nullable: false),
                    VeilingID = table.Column<int>(type: "int", nullable: false),
                    GebeurtenisDatum = table.Column<DateTime>(type: "datetime2", nullable: true),
                    GewensteLocatie = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    KoperDBID = table.Column<int>(type: "int", nullable: true),
                    VeilingDBID = table.Column<int>(type: "int", nullable: true),
                    productDBID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Lots", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Lots_Gebruikers_KoperDBID",
                        column: x => x.KoperDBID,
                        principalTable: "Gebruikers",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_Lots_Veilingen_VeilingDBID",
                        column: x => x.VeilingDBID,
                        principalTable: "Veilingen",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_Lots_product_productDBID",
                        column: x => x.productDBID,
                        principalTable: "product",
                        principalColumn: "ID");
                });

            migrationBuilder.InsertData(
                table: "Gebruikers",
                columns: new[] { "ID", "Discriminator", "Email", "Naam", "Paswoord", "Rol", "Telefoonnummer" },
                values: new object[] { 1, "GebruikerDB", "admin@example.com", "Admin", "1234", "Administrator", 123456789 });

            migrationBuilder.CreateIndex(
                name: "IX_Lots_KoperDBID",
                table: "Lots",
                column: "KoperDBID");

            migrationBuilder.CreateIndex(
                name: "IX_Lots_productDBID",
                table: "Lots",
                column: "productDBID");

            migrationBuilder.CreateIndex(
                name: "IX_Lots_VeilingDBID",
                table: "Lots",
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
                name: "Lots");

            migrationBuilder.DropTable(
                name: "Veilingen");

            migrationBuilder.DropTable(
                name: "product");

            migrationBuilder.DropTable(
                name: "Gebruikers");
        }
    }
}
