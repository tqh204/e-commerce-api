using Domain.Enums;

namespace Domain.Entities
{
    public class PromotionRule
    {
        public Guid ruleId { get; set; }
        public string ruleName { get; set; } = default!;
        public PromotionRuleType ruleType { get; set; }
        public int priority { get; set; }
        public bool isActive { get; set; } = true;
        public bool isDeleted { get; set; } = false;
        public DateTime startDate { get; set; }
        public DateTime endDate { get; set; }
        public DateTime createdAt { get; set; } = DateTime.UtcNow;
        public DateTime? updatedAt { get; set; }

        public PromotionRuleCondition condition { get; set; } = default!;
        public PromotionRuleBenefit benefit { get; set; } = default!;
    }
}
