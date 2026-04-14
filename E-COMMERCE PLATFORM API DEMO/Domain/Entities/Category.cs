using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entities
{
    public class Category
    {
        public int categoryId {  get; set; }
        public string categoryName { get; set; }
        public ICollection<Product> Products { get; set; } = new List<Product>();

    }
}
