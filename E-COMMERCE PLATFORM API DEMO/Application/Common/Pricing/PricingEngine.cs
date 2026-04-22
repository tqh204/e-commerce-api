using Application.Common.Pricing;
using Application.Interfaces;
using Domain.Entities;
using Domain.Enums;

namespace Application.Common.Pricing
{
    public class PricingEngine : IPricingEngine
    {
        private readonly IPromotionRuleRepository _promotionRuleRepository;

        public PricingEngine(IPromotionRuleRepository promotionRuleRepository)
        {
            _promotionRuleRepository = promotionRuleRepository;
        }

        public async Task<PricingResult> CalculatePromotionAsync(IReadOnlyList<CartItem> items, DateTime now, CancellationToken cancellationToken = default)
        {
            if (items == null || items.Count == 0)//Check if items is null or empty
            {
                return new PricingResult();
            }

            var activeRules = await _promotionRuleRepository.GetActiveRulesAsync(now);//Taking all rules that are active
            if (activeRules.Count == 0)//Check if there are any active rules
            {
                return new PricingResult();
            }

            var cartSubTotal = items.Sum(i => i.unitPrice * i.quantity);//Calculating the total price of the items
            if (cartSubTotal <= 0)//Check if the total price is greater than 0
            {
                return new PricingResult();
            }

            var evaluated = activeRules.Select(rule => new//Starting a loop to check whether any rule is applicable
                {
                    rule,
                    discount = CalculateRuleDiscount(rule, items, cartSubTotal)
                }).Where(x => x.discount > 0).OrderByDescending(x => x.discount).ThenBy(x => x.rule.priority).FirstOrDefault();

            if (evaluated == null)
            {
                return new PricingResult();
            }

            return new PricingResult
            {
                appliedRuleId = evaluated.rule.ruleId,
                appliedRuleName = evaluated.rule.ruleName,
                appliedRuleType = evaluated.rule.ruleType,
                appliedPriority = evaluated.rule.priority,
                discountAmount = evaluated.discount
            };
        }

        private decimal CalculateRuleDiscount(PromotionRule rule, IReadOnlyList<CartItem> items, decimal cartSubTotal)
        {
            if (rule.minOrderValue.HasValue && cartSubTotal < rule.minOrderValue.Value)
            {
                return 0;
            }

            decimal discount = rule.ruleType switch
            {
                PromotionRuleType.BuyXGetY => CalculateBuyXGetYDiscount(rule, items),
                PromotionRuleType.CategoryDiscount => CalculateCategoryDiscount(rule, items),
                _ => 0
            };

            if (rule.maxDiscountAmount.HasValue)
            {
                discount = Math.Min(discount, rule.maxDiscountAmount.Value);
            }

            discount = Math.Max(0, discount);
            discount = Math.Min(discount, cartSubTotal);
            return discount;
        }

        private decimal CalculateBuyXGetYDiscount(PromotionRule rule, IReadOnlyList<CartItem> items)
        {
            if (!rule.buyProductId.HasValue || !rule.getProductId.HasValue ||
                !rule.buyQuantity.HasValue || !rule.getQuantity.HasValue)
            {
                return 0;
            }

            if (rule.buyQuantity.Value <= 0 || rule.getQuantity.Value <= 0)
            {
                return 0;
            }

            var buyItem = items.FirstOrDefault(i => i.productId == rule.buyProductId.Value);
            var getItem = items.FirstOrDefault(i => i.productId == rule.getProductId.Value);

            if (buyItem == null || getItem == null)
            {
                return 0;
            }

            var buyTimes = buyItem.quantity / rule.buyQuantity.Value;
            var getTimes = getItem.quantity / rule.getQuantity.Value;
            var applyTimes = Math.Min(buyTimes, getTimes);

            if (applyTimes <= 0)
            {
                return 0;
            }

            var freeUnits = applyTimes * rule.getQuantity.Value;
            var discountBase = freeUnits * getItem.unitPrice;

            return rule.discountType switch
            {
                DiscountType.Percentage => discountBase * (rule.value / 100m),
                DiscountType.FixedAmount => Math.Min(rule.value, discountBase),
                _ => 0
            };
        }

        private decimal CalculateCategoryDiscount(PromotionRule rule, IReadOnlyList<CartItem> items)
        {
            if (!rule.targetCategoryId.HasValue)
            {
                return 0;
            }

            var categoryTotal = items
                .Where(i => i.product != null && i.product.categoryId == rule.targetCategoryId.Value)
                .Sum(i => i.unitPrice * i.quantity);

            if (categoryTotal <= 0)
            {
                return 0;
            }

            return rule.discountType switch
            {
                DiscountType.Percentage => categoryTotal * (rule.value / 100m),
                DiscountType.FixedAmount => Math.Min(rule.value, categoryTotal),
                _ => 0
            };
        }
    }
}
