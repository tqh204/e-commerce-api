using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class updateTask7variants : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Variants_sku",
                table: "Variants");

            migrationBuilder.CreateIndex(
                name: "IX_Variants_sku",
                table: "Variants",
                column: "sku",
                unique: true,
                filter: "[isDeleted] = 0");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Variants_sku",
                table: "Variants");

            migrationBuilder.CreateIndex(
                name: "IX_Variants_sku",
                table: "Variants",
                column: "sku",
                unique: true);
        }
    }
}
