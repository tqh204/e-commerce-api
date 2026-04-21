using Application.Interfaces;
using Domain.Entities;
using MediatR;

namespace Application.Features.Promotion.Commands
{
    public class DeletePromotionRuleCommandHandler : IRequestHandler<DeletePromotionRuleCommand, Result<bool>>
    {
        private readonly IPromotionRuleRepository _promotionRuleRepository;
        private readonly IUnitOfWork _unitOfWork;

        public DeletePromotionRuleCommandHandler(IPromotionRuleRepository promotionRuleRepository, IUnitOfWork unitOfWork)
        {
            _promotionRuleRepository = promotionRuleRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<bool>> Handle(DeletePromotionRuleCommand request, CancellationToken cancellationToken)
        {
            var rule = await _promotionRuleRepository.GetByIdAsync(request.ruleId);
            if (rule == null)
            {
                return Result<bool>.Failure("Không tìm thấy promotion rule.");
            }

            _promotionRuleRepository.SoftDelete(rule);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result<bool>.Success(true);
        }
    }
}
