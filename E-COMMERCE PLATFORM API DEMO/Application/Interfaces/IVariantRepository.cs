using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces
{
    public interface IVariantRepository
    {
        Task AddAsync(Variant variant);
        void Update(Variant variant);
        void SoftDelete(Variant variant);
        Task<Variant?> GetByIdAsync(Guid variantId);
        Task<(IReadOnlyList<Variant> Items, int TotalCount)> GetByProductIdPagedAsync(Guid productId, int page, int size);
        Task<bool> ExistsSkuAsync(string sku, Guid? excludeVariantId = null);
        Task<bool> ProductExistsAsync(Guid productId);
        Task<int> GetAllocatedInventoryAsync(Guid productId, Guid? excludeVariantId = null);
    }
}
