using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Interfaces
{
    public interface ICategoryRepository
    {
        Task<Category?> GetCategoryById(int categoryId);
        Task<bool> IsCategoryExsits(int categoryId);
    }
}
