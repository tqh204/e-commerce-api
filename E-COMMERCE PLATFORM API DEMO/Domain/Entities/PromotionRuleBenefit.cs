using Domain.Enums;

namespace Domain.Entities
{
    public class PromotionRuleBenefit
    {
        public Guid benefitId { get; set; }
        public Guid ruleId { get; set; }
        public PromotionRule rule { get; set; } = default!;
        public DiscountType discountType { get; set; }
        public decimal value { get; set; }
        public decimal? maxDiscountAmount { get; set; }
    }
}
