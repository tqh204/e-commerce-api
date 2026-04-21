using FluentValidation;

namespace Application.Features.Promotion.Commands
{
    public class DeletePromotionRuleCommandValidator : AbstractValidator<DeletePromotionRuleCommand>
    {
        public DeletePromotionRuleCommandValidator()
        {
            RuleFor(x => x.ruleId)
                .NotEmpty().WithMessage("RuleId không hợp lệ");
        }
    }
}
