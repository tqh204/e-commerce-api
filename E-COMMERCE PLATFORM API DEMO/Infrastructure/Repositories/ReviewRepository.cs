using Application.Interfaces;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Repositories
{
    public class ReviewRepository : IReviewRepository
    {
        private readonly AppDBContext _context;
        public ReviewRepository(AppDBContext context) => _context = context;
        public async Task<bool> HasCompletedOrderWithProductAsync(Guid userId, Guid productId)
        {
            // Kiểm tra user có order COMPLETED nào chứa productId không
            return await _context.Orders.Where(o => o.userId == userId && o.status == OrderStatus.Completed).AnyAsync(o => o.items.Any(i => i.productId == productId));

        }
        public async Task<bool> HasAlreadyReviewedAsync(Guid userId, Guid productId)
        {
            return await _context.Reviews.AnyAsync(r => r.userId == userId && r.productId == productId);

        }
        public async Task AddAsync(Review review)
        {
            await _context.Reviews.AddAsync(review);
        }
        public async Task<(IReadOnlyList<Review> items, int TotalCount)> GetByProductIdAsync(
            Guid productId, int page, int size)
        {
            var query = _context.Reviews
                .Include(r => r.user)
                .Where(r => r.productId == productId)
                .OrderByDescending(r => r.createdAt);
            var totalCount = await query.CountAsync();
            var items = await query
                .Skip((page - 1) * size)
                .Take(size)
                .ToListAsync();
            return (items, totalCount);
        }
    }
}
