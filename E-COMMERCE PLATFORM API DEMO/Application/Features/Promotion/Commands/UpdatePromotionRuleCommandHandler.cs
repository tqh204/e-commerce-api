using Application.Interfaces;
using Domain.Entities;
using Domain.Enums;
using MediatR;

namespace Application.Features.Promotion.Commands
{
    public class UpdatePromotionRuleCommandHandler : IRequestHandler<UpdatePromotionRuleCommand, Result<bool>>
    {
        private readonly IPromotionRuleRepository _promotionRuleRepository;
        private readonly IUnitOfWork _unitOfWork;

        public UpdatePromotionRuleCommandHandler(IPromotionRuleRepository promotionRuleRepository, IUnitOfWork unitOfWork)
        {
            _promotionRuleRepository = promotionRuleRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<bool>> Handle(UpdatePromotionRuleCommand request, CancellationToken cancellationToken)
        {
            var rule = await _promotionRuleRepository.GetByIdAsync(request.ruleId);
            if (rule == null)
            {
                return Result<bool>.Failure("Không tìm thấy promotion rule.");
            }

            if (request.ruleType == PromotionRuleType.BuyXGetY && request.targetCategoryId.HasValue)
            {
                return Result<bool>.Failure("BuyXGetY không được truyền TargetCategoryId.");
            }

            if (request.ruleType == PromotionRuleType.CategoryDiscount &&
                (request.buyProductId.HasValue || request.getProductId.HasValue || request.buyQuantity.HasValue || request.getQuantity.HasValue))
            {
                return Result<bool>.Failure("CategoryDiscount không được truyền thuộc tính BuyXGetY.");
            }

            rule.ruleName = request.ruleName.Trim();
            rule.ruleType = request.ruleType;
            rule.priority = request.priority;
            rule.isActive = request.isActive;
            rule.startDate = request.startDate;
            rule.endDate = request.endDate;
            rule.updatedAt = DateTime.UtcNow;

            rule.condition.buyProductId = request.buyProductId;
            rule.condition.buyQuantity = request.buyQuantity;
            rule.condition.getProductId = request.getProductId;
            rule.condition.getQuantity = request.getQuantity;
            rule.condition.targetCategoryId = request.targetCategoryId;
            rule.condition.minOrderValue = request.minOrderValue;

            rule.benefit.discountType = request.discountType;
            rule.benefit.value = request.value;
            rule.benefit.maxDiscountAmount = request.maxDiscountAmount;

            _promotionRuleRepository.Update(rule);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result<bool>.Success(true);
        }
    }
}
