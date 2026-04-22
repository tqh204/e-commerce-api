using Domain.Enums;

namespace Application.Common.Pricing
{
    public class PricingResult
    {
        public Guid? appliedRuleId { get; set; }
        public string? appliedRuleName { get; set; }
        public PromotionRuleType? appliedRuleType { get; set; }
        public int appliedPriority { get; set; }
        public decimal discountAmount { get; set; }
    }
}
