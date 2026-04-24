using System;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    [DbContext(typeof(AppDBContext))]
    [Migration("20260423113000_Task10_PayOS")]
    public partial class Task10_PayOS : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Payments",
                columns: table => new
                {
                    paymentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    userId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    orderCode = table.Column<long>(type: "bigint", nullable: false),
                    amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    description = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    paymentLinkId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    checkoutUrl = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    providerStatus = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    providerReference = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    transactionDateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    linkedOrderId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    linkedShipmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    returnUrl = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    cancelUrl = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    cartSnapshotJson = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    selectedShipmentJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    failureReason = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    createdAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    paidAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Payments", x => x.paymentId);
                    table.ForeignKey(
                        name: "FK_Payments_Orders_linkedOrderId",
                        column: x => x.linkedOrderId,
                        principalTable: "Orders",
                        principalColumn: "orderId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Payments_Shipments_linkedShipmentId",
                        column: x => x.linkedShipmentId,
                        principalTable: "Shipments",
                        principalColumn: "shipmentId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Payments_Users_userId",
                        column: x => x.userId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Payments_linkedOrderId",
                table: "Payments",
                column: "linkedOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_linkedShipmentId",
                table: "Payments",
                column: "linkedShipmentId");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_orderCode",
                table: "Payments",
                column: "orderCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Payments_userId",
                table: "Payments",
                column: "userId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Payments");
        }
    }
}
