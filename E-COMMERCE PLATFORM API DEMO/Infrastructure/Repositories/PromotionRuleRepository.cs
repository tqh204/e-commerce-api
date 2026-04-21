using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class PromotionRuleRepository : IPromotionRuleRepository
    {
        private readonly AppDBContext _context;

        public PromotionRuleRepository(AppDBContext context)
        {
            _context = context;
        }

        public async Task<PromotionRule?> GetByIdAsync(Guid ruleId)
        {
            return await _context.PromotionRules
                .Include(r => r.condition)
                .Include(r => r.benefit)
                .FirstOrDefaultAsync(r => !r.isDeleted && r.ruleId == ruleId);
        }

        public async Task<IReadOnlyList<PromotionRule>> GetActiveRulesAsync(DateTime now)
        {
            return await _context.PromotionRules
                .Include(r => r.condition)
                .Include(r => r.benefit)
                .Where(r =>
                    !r.isDeleted &&
                    r.isActive &&
                    r.startDate <= now &&
                    r.endDate >= now)
                .OrderBy(r => r.priority)
                .ToListAsync();
        }

        public async Task AddAsync(PromotionRule rule)
        {
            await _context.PromotionRules.AddAsync(rule);
        }

        public void Update(PromotionRule rule)
        {
            _context.PromotionRules.Update(rule);
        }

        public void SoftDelete(PromotionRule rule)
        {
            rule.isDeleted = true;
            rule.updatedAt = DateTime.UtcNow;
            _context.PromotionRules.Update(rule);
        }
    }
}
