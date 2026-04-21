using Application.Features.Pricing.Common;
using Application.Interfaces;
using Domain.Entities;
using Domain.Enums;

namespace Application.Features.Pricing
{
    public class PricingEngine : IPricingEngine
    {
        private readonly IPromotionRuleRepository _promotionRuleRepository;

        public PricingEngine(IPromotionRuleRepository promotionRuleRepository)
        {
            _promotionRuleRepository = promotionRuleRepository;
        }

        public async Task<PricingResult> CalculatePromotionAsync(
            IReadOnlyList<CartItem> items,
            DateTime now,
            CancellationToken cancellationToken = default)
        {
            if (items == null || items.Count == 0)
            {
                return new PricingResult();
            }

            var activeRules = await _promotionRuleRepository.GetActiveRulesAsync(now);
            if (activeRules.Count == 0)
            {
                return new PricingResult();
            }

            var cartSubTotal = items.Sum(i => i.unitPrice * i.quantity);
            if (cartSubTotal <= 0)
            {
                return new PricingResult();
            }

            var evaluated = activeRules
                .Select(rule => new
                {
                    rule,
                    discount = CalculateRuleDiscount(rule, items, cartSubTotal)
                })
                .Where(x => x.discount > 0)
                .OrderByDescending(x => x.discount)
                .ThenBy(x => x.rule.priority)
                .FirstOrDefault();

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
            if (rule.condition.minOrderValue.HasValue && cartSubTotal < rule.condition.minOrderValue.Value)
            {
                return 0;
            }

            decimal discount = rule.ruleType switch
            {
                PromotionRuleType.BuyXGetY => CalculateBuyXGetYDiscount(rule, items),
                PromotionRuleType.CategoryDiscount => CalculateCategoryDiscount(rule, items),
                _ => 0
            };

            if (rule.benefit.maxDiscountAmount.HasValue)
            {
                discount = Math.Min(discount, rule.benefit.maxDiscountAmount.Value);
            }

            discount = Math.Max(0, discount);
            discount = Math.Min(discount, cartSubTotal);
            return discount;
        }

        private decimal CalculateBuyXGetYDiscount(PromotionRule rule, IReadOnlyList<CartItem> items)
        {
            if (!rule.condition.buyProductId.HasValue || !rule.condition.getProductId.HasValue ||
                !rule.condition.buyQuantity.HasValue || !rule.condition.getQuantity.HasValue)
            {
                return 0;
            }

            if (rule.condition.buyQuantity.Value <= 0 || rule.condition.getQuantity.Value <= 0)
            {
                return 0;
            }

            var buyItem = items.FirstOrDefault(i => i.productId == rule.condition.buyProductId.Value);
            var getItem = items.FirstOrDefault(i => i.productId == rule.condition.getProductId.Value);

            if (buyItem == null || getItem == null)
            {
                return 0;
            }

            var buyTimes = buyItem.quantity / rule.condition.buyQuantity.Value;
            var getTimes = getItem.quantity / rule.condition.getQuantity.Value;
            var applyTimes = Math.Min(buyTimes, getTimes);

            if (applyTimes <= 0)
            {
                return 0;
            }

            var freeUnits = applyTimes * rule.condition.getQuantity.Value;
            var discountBase = freeUnits * getItem.unitPrice;

            return rule.benefit.discountType switch
            {
                DiscountType.Percentage => discountBase * (rule.benefit.value / 100m),
                DiscountType.FixedAmount => Math.Min(rule.benefit.value, discountBase),
                _ => 0
            };
        }

        private decimal CalculateCategoryDiscount(PromotionRule rule, IReadOnlyList<CartItem> items)
        {
            if (!rule.condition.targetCategoryId.HasValue)
            {
                return 0;
            }

            var categoryTotal = items
                .Where(i => i.product != null && i.product.categoryId == rule.condition.targetCategoryId.Value)
                .Sum(i => i.unitPrice * i.quantity);

            if (categoryTotal <= 0)
            {
                return 0;
            }

            return rule.benefit.discountType switch
            {
                DiscountType.Percentage => categoryTotal * (rule.benefit.value / 100m),
                DiscountType.FixedAmount => Math.Min(rule.benefit.value, categoryTotal),
                _ => 0
            };
        }
    }
}
