using Domain.Entities;

namespace Application.Interfaces
{
    public interface IPromotionRuleRepository
    {
        Task<PromotionRule?> GetByIdAsync(Guid ruleId);//Get rule by Id
        Task<IReadOnlyList<PromotionRule>> GetActiveRulesAsync(DateTime now);//Get active rules by now
        Task AddAsync(PromotionRule rule);
        void Update(PromotionRule rule);
        void SoftDelete(PromotionRule rule);
    }
}
