using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly AppDBContext _context;
        public ProductRepository(AppDBContext context) => _context = context;
        public async Task AddAsync(Product product) => await _context.Products.AddAsync(product);


        public async Task<(IReadOnlyList<Product> Items, int TotalCount)> GetPagedAsync(int page, int size, int? categoryId, decimal? minPrice, decimal? maxPrice)
        {
            var query = _context.Products.Include(u => u.category).Where(u => !u.isDeleted).AsQueryable();//Starting at Product table and taking categories atrributes associated and used where to get product which is active. Finally AsQueryable to use connect to another query
            if (categoryId.HasValue)
            {
                query = query.Where(u => u.categoryId == categoryId.Value);//Filter by CategoryId if client transmitted
            }
            if (minPrice.HasValue)
            {
                query = query.Where(u => u.price >= minPrice.Value);//filter by minPrice
            }
            if (maxPrice.HasValue)
            {
                query = query.Where(u => u.price <= maxPrice.Value);//Filter by maxPrice
            }
            var totalCount = await query.CountAsync();//Count record after filter

            var items = await query.Skip((page - 1) * size).Take(size).ToListAsync();//Apply the pagination.Page = 1, size = 10 is product amount in 1 page. If there is not any value transmitted to Skip that mean Skip = 0 and it would take 10 record = 10 product in that page. And if page = 2 mean skip = 2 - 1 = 1 and it would skip 1 page to take next 10 record in page 2
            return (items, totalCount);
        }
        public async Task<Product?> GetProductByIdAsync(Guid productId) => await _context.Products.Include(x => x.category).Where(u => !u.isDeleted).FirstOrDefaultAsync(u => u.productId == productId);
        

        public void SoftDelete(Product product)
        {
            product.isDeleted = true;
            product.updatedAt = DateTime.UtcNow;
            _context.Products.Update(product);
        }

        public void Update(Product product)
        {
            _context.Products.Update(product);
        }
    }
}
