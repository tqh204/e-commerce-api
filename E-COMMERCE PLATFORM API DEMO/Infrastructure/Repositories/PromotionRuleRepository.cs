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

        public async Task<PromotionRule?> GetByIdAsync(Guid ruleId) => await _context.PromotionRules.FirstOrDefaultAsync(r => !r.isDeleted && r.ruleId == ruleId);//Lấy rule theo Id

        public async Task<IReadOnlyList<PromotionRule>> GetActiveRulesAsync(DateTime now) => await _context.PromotionRules.Where(r => !r.isDeleted && r.isActive && r.startDate <= now && r.endDate >= now).OrderBy(r => r.priority).ToListAsync();//Check rule còn hoạt động và có tồn tại



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
