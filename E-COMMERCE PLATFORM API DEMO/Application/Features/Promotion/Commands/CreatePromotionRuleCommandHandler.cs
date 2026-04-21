using Application.Interfaces;
using Domain.Entities;
using Domain.Enums;
using MediatR;

namespace Application.Features.Promotion.Commands
{
    public class CreatePromotionRuleCommandHandler : IRequestHandler<CreatePromotionRuleCommand, Result<Guid>>
    {
        private readonly IPromotionRuleRepository _promotionRuleRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CreatePromotionRuleCommandHandler(IPromotionRuleRepository promotionRuleRepository, IUnitOfWork unitOfWork)
        {
            _promotionRuleRepository = promotionRuleRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<Guid>> Handle(CreatePromotionRuleCommand request, CancellationToken cancellationToken)
        {
            if (request.ruleType == PromotionRuleType.BuyXGetY && request.targetCategoryId.HasValue)
            {
                return Result<Guid>.Failure("BuyXGetY không được truyền TargetCategoryId.");
            }

            if (request.ruleType == PromotionRuleType.CategoryDiscount &&
                (request.buyProductId.HasValue || request.getProductId.HasValue || request.buyQuantity.HasValue || request.getQuantity.HasValue))
            {
                return Result<Guid>.Failure("CategoryDiscount không được truyền thuộc tính BuyXGetY.");
            }

            var ruleId = Guid.NewGuid();
            var rule = new PromotionRule
            {
                ruleId = ruleId,
                ruleName = request.ruleName.Trim(),
                ruleType = request.ruleType,
                priority = request.priority,
                isActive = request.isActive,
                isDeleted = false,
                startDate = request.startDate,
                endDate = request.endDate,
                createdAt = DateTime.UtcNow
            };

            rule.condition = new PromotionRuleCondition
            {
                conditionId = Guid.NewGuid(),
                ruleId = ruleId,
                buyProductId = request.buyProductId,
                buyQuantity = request.buyQuantity,
                getProductId = request.getProductId,
                getQuantity = request.getQuantity,
                targetCategoryId = request.targetCategoryId,
                minOrderValue = request.minOrderValue
            };

            rule.benefit = new PromotionRuleBenefit
            {
                benefitId = Guid.NewGuid(),
                ruleId = ruleId,
                discountType = request.discountType,
                value = request.value,
                maxDiscountAmount = request.maxDiscountAmount
            };

            await _promotionRuleRepository.AddAsync(rule);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<Guid>.Success(ruleId);
        }
    }
}
