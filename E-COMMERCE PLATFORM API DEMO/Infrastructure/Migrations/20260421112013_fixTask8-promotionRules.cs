using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class fixTask8promotionRules : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PromotionRuleBenefits");

            migrationBuilder.DropTable(
                name: "PromotionRuleConditions");

            migrationBuilder.AddColumn<Guid>(
                name: "buyProductId",
                table: "PromotionRules",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "buyQuantity",
                table: "PromotionRules",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "discountType",
                table: "PromotionRules",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<Guid>(
                name: "getProductId",
                table: "PromotionRules",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "getQuantity",
                table: "PromotionRules",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "maxDiscountAmount",
                table: "PromotionRules",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "minOrderValue",
                table: "PromotionRules",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "targetCategoryId",
                table: "PromotionRules",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "value",
                table: "PromotionRules",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "buyProductId",
                table: "PromotionRules");

            migrationBuilder.DropColumn(
                name: "buyQuantity",
                table: "PromotionRules");

            migrationBuilder.DropColumn(
                name: "discountType",
                table: "PromotionRules");

            migrationBuilder.DropColumn(
                name: "getProductId",
                table: "PromotionRules");

            migrationBuilder.DropColumn(
                name: "getQuantity",
                table: "PromotionRules");

            migrationBuilder.DropColumn(
                name: "maxDiscountAmount",
                table: "PromotionRules");

            migrationBuilder.DropColumn(
                name: "minOrderValue",
                table: "PromotionRules");

            migrationBuilder.DropColumn(
                name: "targetCategoryId",
                table: "PromotionRules");

            migrationBuilder.DropColumn(
                name: "value",
                table: "PromotionRules");

            migrationBuilder.CreateTable(
                name: "PromotionRuleBenefits",
                columns: table => new
                {
                    benefitId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ruleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    discountType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    maxDiscountAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    value = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PromotionRuleBenefits", x => x.benefitId);
                    table.ForeignKey(
                        name: "FK_PromotionRuleBenefits_PromotionRules_ruleId",
                        column: x => x.ruleId,
                        principalTable: "PromotionRules",
                        principalColumn: "ruleId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PromotionRuleConditions",
                columns: table => new
                {
                    conditionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ruleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    buyProductId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    buyQuantity = table.Column<int>(type: "int", nullable: true),
                    getProductId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    getQuantity = table.Column<int>(type: "int", nullable: true),
                    minOrderValue = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    targetCategoryId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PromotionRuleConditions", x => x.conditionId);
                    table.ForeignKey(
                        name: "FK_PromotionRuleConditions_PromotionRules_ruleId",
                        column: x => x.ruleId,
                        principalTable: "PromotionRules",
                        principalColumn: "ruleId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PromotionRuleBenefits_ruleId",
                table: "PromotionRuleBenefits",
                column: "ruleId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PromotionRuleConditions_ruleId",
                table: "PromotionRuleConditions",
                column: "ruleId",
                unique: true);
        }
    }
}
