using Domain.Entities;

namespace Application.Interfaces
{
    public interface IPromotionRuleRepository
    {
        Task<PromotionRule?> GetByIdAsync(Guid ruleId);
        Task<IReadOnlyList<PromotionRule>> GetActiveRulesAsync(DateTime now);
        Task AddAsync(PromotionRule rule);
        void Update(PromotionRule rule);
        void SoftDelete(PromotionRule rule);
    }
}
