using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebProject_klas3_groep4.Migrations
{
    /// <inheritdoc />
    public partial class SeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Gebruikers",
                columns: new[] { "ID", "Discriminator", "Email", "Naam", "Rol", "Telefoonnummer" },
                values: new object[] { 1, "GebruikerDB", "admin@example.com", "Admin", "Administrator", 123456789 });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Gebruikers",
                keyColumn: "ID",
                keyValue: 1);
        }
    }
}
