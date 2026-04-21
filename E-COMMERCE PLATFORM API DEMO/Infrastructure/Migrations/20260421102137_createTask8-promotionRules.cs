using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class createTask8promotionRules : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PromotionRules",
                columns: table => new
                {
                    ruleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ruleName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    ruleType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    priority = table.Column<int>(type: "int", nullable: false),
                    isActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    isDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    startDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    endDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    createdAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PromotionRules", x => x.ruleId);
                });

            migrationBuilder.CreateTable(
                name: "PromotionRuleBenefits",
                columns: table => new
                {
                    benefitId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ruleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    discountType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    value = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    maxDiscountAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: true)
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
                    targetCategoryId = table.Column<int>(type: "int", nullable: true),
                    minOrderValue = table.Column<decimal>(type: "decimal(18,2)", nullable: true)
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

            migrationBuilder.CreateIndex(
                name: "IX_PromotionRules_isActive_isDeleted_startDate_endDate_priority",
                table: "PromotionRules",
                columns: new[] { "isActive", "isDeleted", "startDate", "endDate", "priority" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PromotionRuleBenefits");

            migrationBuilder.DropTable(
                name: "PromotionRuleConditions");

            migrationBuilder.DropTable(
                name: "PromotionRules");
        }
    }
}
