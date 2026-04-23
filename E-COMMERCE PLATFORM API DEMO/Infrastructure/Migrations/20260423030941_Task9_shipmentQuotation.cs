using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Task9_shipmentQuotation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ShipmentQuotations",
                columns: table => new
                {
                    shipmentQuotationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    quotationId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    fee = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    currency = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    expiresAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    serviceType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    pickupStopId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    dropoffStopId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    pickupAddress = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    pickupLat = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    pickupLng = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    dropoffAddress = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    dropoffLat = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    dropoffLng = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    packageWeight = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    packageLength = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    packageWidth = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    packageHeight = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    packageQuantity = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    packageCategory = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    createdAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShipmentQuotations", x => x.shipmentQuotationId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ShipmentQuotations_quotationId",
                table: "ShipmentQuotations",
                column: "quotationId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ShipmentQuotations");
        }
    }
}
