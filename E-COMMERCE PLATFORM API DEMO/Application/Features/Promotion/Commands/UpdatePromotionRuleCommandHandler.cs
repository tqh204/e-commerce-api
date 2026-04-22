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

            rule.buyProductId = request.buyProductId;
            rule.buyQuantity = request.buyQuantity;
            rule.getProductId = request.getProductId;
            rule.getQuantity = request.getQuantity;
            rule.targetCategoryId = request.targetCategoryId;
            rule.minOrderValue = request.minOrderValue;

            rule.discountType = request.discountType;
            rule.value = request.value;
            rule.maxDiscountAmount = request.maxDiscountAmount;

            _promotionRuleRepository.Update(rule);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result<bool>.Success(true);
        }
    }
}
