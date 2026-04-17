using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Task5coupon2applycoupon : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "couponCode",
                table: "Carts",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "couponId",
                table: "Carts",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "discountAmount",
                table: "Carts",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateIndex(
                name: "IX_Carts_couponId",
                table: "Carts",
                column: "couponId");

            migrationBuilder.AddForeignKey(
                name: "FK_Carts_Coupons_couponId",
                table: "Carts",
                column: "couponId",
                principalTable: "Coupons",
                principalColumn: "couponId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Carts_Coupons_couponId",
                table: "Carts");

            migrationBuilder.DropIndex(
                name: "IX_Carts_couponId",
                table: "Carts");

            migrationBuilder.DropColumn(
                name: "couponCode",
                table: "Carts");

            migrationBuilder.DropColumn(
                name: "couponId",
                table: "Carts");

            migrationBuilder.DropColumn(
                name: "discountAmount",
                table: "Carts");
        }
    }
}
