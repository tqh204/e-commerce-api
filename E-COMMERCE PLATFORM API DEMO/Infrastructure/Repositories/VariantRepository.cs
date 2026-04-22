using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Repositories
{
    public class VariantRepository : IVariantRepository
    {
        private readonly AppDBContext _context;

        public VariantRepository(AppDBContext context) => _context = context;
        
        public async Task AddAsync(Variant variant) => await _context.Variants.AddAsync(variant);

        public async Task<Variant?> GetByIdAsync(Guid variantId) => await _context.Variants.Where(v => !v.isDeleted).FirstOrDefaultAsync(v => v.variantId == variantId);

        public async Task<(IReadOnlyList<Variant> Items, int TotalCount)> GetByProductIdPagedAsync(Guid productId, int page, int size)
        {
            var query = _context.Variants.Where(v => !v.isDeleted && v.productId == productId).OrderByDescending(v => v.createdAt);

            var totalCount = await query.CountAsync();

            var items = await query.Skip((page - 1) * size).Take(size).ToListAsync();

            return (items, totalCount);
        }
        public async Task<bool> ExistsSkuAsync(string sku, Guid? excludeVariantId = null) => await _context.Variants.AnyAsync(v => !v.isDeleted && v.sku == sku &&(!excludeVariantId.HasValue || v.variantId != excludeVariantId.Value));

        public async Task<bool> ProductExistsAsync(Guid productId) => await _context.Products.AnyAsync(p => !p.isDeleted && p.productId == productId);

        public void Update(Variant variant) => _context.Variants.Update(variant);

        public void SoftDelete(Variant variant)
        {
            variant.isDeleted = true;
            variant.updatedAt = DateTime.UtcNow;
            _context.Variants.Update(variant);
        }

        public async Task<int> GetAllocatedInventoryAsync(Guid productId, Guid? excludeVariantId = null)
        {
            var query = _context.Variants.Where(v => !v.isDeleted && v.productId == productId);

            if (excludeVariantId.HasValue)
                query = query.Where(v => v.variantId != excludeVariantId.Value);

            return await query.SumAsync(v => v.inventory);
        }
    }
}
