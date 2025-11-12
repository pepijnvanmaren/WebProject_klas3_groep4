using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebProject_klas3_groep4.Migrations
{
    /// <inheritdoc />
    public partial class password : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Lot_Gebruikers_KoperDBID",
                table: "Lot");

            migrationBuilder.DropForeignKey(
                name: "FK_Lot_Veilingen_VeilingDBID",
                table: "Lot");

            migrationBuilder.DropForeignKey(
                name: "FK_Lot_product_ProductID",
                table: "Lot");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Lot",
                table: "Lot");

            migrationBuilder.DropIndex(
                name: "IX_Lot_ProductID",
                table: "Lot");

            migrationBuilder.RenameTable(
                name: "Lot",
                newName: "Lots");

            migrationBuilder.RenameIndex(
                name: "IX_Lot_VeilingDBID",
                table: "Lots",
                newName: "IX_Lots_VeilingDBID");

            migrationBuilder.RenameIndex(
                name: "IX_Lot_KoperDBID",
                table: "Lots",
                newName: "IX_Lots_KoperDBID");

            migrationBuilder.AddColumn<string>(
                name: "Paswoord",
                table: "Gebruikers",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "productDBID",
                table: "Lots",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Lots",
                table: "Lots",
                column: "ID");

            migrationBuilder.UpdateData(
                table: "Gebruikers",
                keyColumn: "ID",
                keyValue: 1,
                column: "Paswoord",
                value: "1234");

            migrationBuilder.CreateIndex(
                name: "IX_Lots_productDBID",
                table: "Lots",
                column: "productDBID");

            migrationBuilder.AddForeignKey(
                name: "FK_Lots_Gebruikers_KoperDBID",
                table: "Lots",
                column: "KoperDBID",
                principalTable: "Gebruikers",
                principalColumn: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK_Lots_Veilingen_VeilingDBID",
                table: "Lots",
                column: "VeilingDBID",
                principalTable: "Veilingen",
                principalColumn: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK_Lots_product_productDBID",
                table: "Lots",
                column: "productDBID",
                principalTable: "product",
                principalColumn: "ID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Lots_Gebruikers_KoperDBID",
                table: "Lots");

            migrationBuilder.DropForeignKey(
                name: "FK_Lots_Veilingen_VeilingDBID",
                table: "Lots");

            migrationBuilder.DropForeignKey(
                name: "FK_Lots_product_productDBID",
                table: "Lots");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Lots",
                table: "Lots");

            migrationBuilder.DropIndex(
                name: "IX_Lots_productDBID",
                table: "Lots");

            migrationBuilder.DropColumn(
                name: "Paswoord",
                table: "Gebruikers");

            migrationBuilder.DropColumn(
                name: "productDBID",
                table: "Lots");

            migrationBuilder.RenameTable(
                name: "Lots",
                newName: "Lot");

            migrationBuilder.RenameIndex(
                name: "IX_Lots_VeilingDBID",
                table: "Lot",
                newName: "IX_Lot_VeilingDBID");

            migrationBuilder.RenameIndex(
                name: "IX_Lots_KoperDBID",
                table: "Lot",
                newName: "IX_Lot_KoperDBID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Lot",
                table: "Lot",
                column: "ID");

            migrationBuilder.CreateIndex(
                name: "IX_Lot_ProductID",
                table: "Lot",
                column: "ProductID");

            migrationBuilder.AddForeignKey(
                name: "FK_Lot_Gebruikers_KoperDBID",
                table: "Lot",
                column: "KoperDBID",
                principalTable: "Gebruikers",
                principalColumn: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK_Lot_Veilingen_VeilingDBID",
                table: "Lot",
                column: "VeilingDBID",
                principalTable: "Veilingen",
                principalColumn: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK_Lot_product_ProductID",
                table: "Lot",
                column: "ProductID",
                principalTable: "product",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
