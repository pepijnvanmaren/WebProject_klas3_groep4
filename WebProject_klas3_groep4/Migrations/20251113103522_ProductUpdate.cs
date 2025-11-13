using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebProject_klas3_groep4.Migrations
{
    /// <inheritdoc />
    public partial class ProductUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Gebruikers",
                keyColumn: "ID",
                keyValue: 1);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Gebruikers",
                columns: new[] { "ID", "Discriminator", "Email", "Naam", "Paswoord", "Rol", "Telefoonnummer" },
                values: new object[] { 1, "GebruikerDB", "admin@example.com", "Admin", "1234", "Administrator", 123456789 });
        }
    }
}
