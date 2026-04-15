using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces
{
    public interface IProductRepository
    {
        Task<(IReadOnlyList<Product> Items, int TotalCount)> GetPagedAsync(
       int page,
       int size,
       int? categoryId,
       decimal? minPrice,
       decimal? maxPrice);//Could create one more entity to contain these attributes and when u create this function u just take the entity is just created 

        Task AddAsync(Product product);
        void Update(Product product);

        Task<Product?> GetProductByIdAsync(Guid guid);
        void SoftDelete(Product product);
    }
}
