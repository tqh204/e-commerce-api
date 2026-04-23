using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Task9shipment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Shipments",
                columns: table => new
                {
                    shipmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    orderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    provider = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    serviceType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    fee = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    currency = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    quotationId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    quotationExpiresAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    providerOrderId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    driverId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    shareLink = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    pickupAddress = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    pickupLat = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    pickupLng = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    dropoffAddress = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    dropoffLat = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    dropoffLng = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    senderName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    senderPhone = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    recipientName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    recipientPhone = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    codAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    lastWebhookEvent = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    lastWebhookPayload = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    lastWebhookAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    failureReason = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    createdAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Shipments", x => x.shipmentId);
                    table.ForeignKey(
                        name: "FK_Shipments_Orders_orderId",
                        column: x => x.orderId,
                        principalTable: "Orders",
                        principalColumn: "orderId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Shipments_orderId",
                table: "Shipments",
                column: "orderId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Shipments");
        }
    }
}
