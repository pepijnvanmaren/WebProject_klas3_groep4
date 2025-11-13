using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebProject_klas3_groep4.Migrations
{
    /// <inheritdoc />
    public partial class product : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "bedrijf",
                table: "product",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "gekocht",
                table: "product",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "hoeveelheid",
                table: "product",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "startprijs",
                table: "product",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "bedrijf",
                table: "product");

            migrationBuilder.DropColumn(
                name: "gekocht",
                table: "product");

            migrationBuilder.DropColumn(
                name: "hoeveelheid",
                table: "product");

            migrationBuilder.DropColumn(
                name: "startprijs",
                table: "product");
        }
    }
}
