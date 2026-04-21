namespace Domain.Entities
{
    public class PromotionRuleCondition
    {
        public Guid conditionId { get; set; }
        public Guid ruleId { get; set; }
        public PromotionRule rule { get; set; } = default!;

        // Buy X Get Y
        public Guid? buyProductId { get; set; }
        public int? buyQuantity { get; set; }
        public Guid? getProductId { get; set; }
        public int? getQuantity { get; set; }

        // Category discount
        public int? targetCategoryId { get; set; }

        // Shared condition
        public decimal? minOrderValue { get; set; }
    }
}
