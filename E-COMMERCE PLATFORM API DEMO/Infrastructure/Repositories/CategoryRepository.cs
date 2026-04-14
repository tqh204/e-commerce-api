using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly AppDBContext _context;

        public CategoryRepository(AppDBContext context) => _context = context;

        public async Task<Category?> GetCategoryById(int categoryId) => await _context.Categories.FirstOrDefaultAsync(u => u.categoryId == categoryId);

        public async Task<bool> IsCategoryExsits(int categoryId) => await _context.Categories.AnyAsync(u => u.categoryId == categoryId);
    }
}
