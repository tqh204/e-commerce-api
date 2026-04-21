using Domain.Enums;
using FluentValidation;

namespace Application.Features.Promotion.Commands
{
    public class CreatePromotionRuleCommandValidator : AbstractValidator<CreatePromotionRuleCommand>
    {
        public CreatePromotionRuleCommandValidator()
        {
            RuleFor(x => x.ruleName)
                .NotEmpty().WithMessage("Tên rule không được để trống")
                .MaximumLength(200).WithMessage("Tên rule tối đa 200 ký tự");

            RuleFor(x => x.ruleType)
                .IsInEnum().WithMessage("RuleType không hợp lệ");

            RuleFor(x => x.priority)
                .GreaterThan(0).WithMessage("Priority phải lớn hơn 0");

            RuleFor(x => x.endDate)
                .GreaterThan(x => x.startDate)
                .WithMessage("EndDate phải lớn hơn StartDate");

            RuleFor(x => x.discountType)
                .IsInEnum().WithMessage("DiscountType không hợp lệ");

            RuleFor(x => x.value)
                .GreaterThan(0).WithMessage("Value phải lớn hơn 0");

            RuleFor(x => x.minOrderValue)
                .GreaterThanOrEqualTo(0).When(x => x.minOrderValue.HasValue)
                .WithMessage("MinOrderValue không được âm");

            RuleFor(x => x.maxDiscountAmount)
                .GreaterThan(0).When(x => x.maxDiscountAmount.HasValue)
                .WithMessage("MaxDiscountAmount phải lớn hơn 0 nếu có truyền");

            When(x => x.ruleType == PromotionRuleType.BuyXGetY, () =>
            {
                RuleFor(x => x.buyProductId)
                    .NotNull().WithMessage("BuyProductId là bắt buộc cho BuyXGetY");

                RuleFor(x => x.getProductId)
                    .NotNull().WithMessage("GetProductId là bắt buộc cho BuyXGetY");

                RuleFor(x => x.buyQuantity)
                    .NotNull().GreaterThan(0).WithMessage("BuyQuantity phải lớn hơn 0 cho BuyXGetY");

                RuleFor(x => x.getQuantity)
                    .NotNull().GreaterThan(0).WithMessage("GetQuantity phải lớn hơn 0 cho BuyXGetY");
            });

            When(x => x.ruleType == PromotionRuleType.CategoryDiscount, () =>
            {
                RuleFor(x => x.targetCategoryId)
                    .NotNull().WithMessage("TargetCategoryId là bắt buộc cho CategoryDiscount");
            });
        }
    }
}
