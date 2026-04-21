using Domain.Entities;
using Domain.Enums;
using MediatR;

namespace Application.Features.Promotion.Commands
{
    public record UpdatePromotionRuleCommand(
        Guid ruleId,
        string ruleName,
        PromotionRuleType ruleType,
        int priority,
        bool isActive,
        DateTime startDate,
        DateTime endDate,
        Guid? buyProductId,
        int? buyQuantity,
        Guid? getProductId,
        int? getQuantity,
        int? targetCategoryId,
        decimal? minOrderValue,
        DiscountType discountType,
        decimal value,
        decimal? maxDiscountAmount
    ) : IRequest<Result<bool>>;
}
