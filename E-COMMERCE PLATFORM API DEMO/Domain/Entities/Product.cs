using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entities
{
    public class Product
    {
        public Guid productId { get; set; }
        public string productName { get; set; }
        public string? description { get; set; }
        public decimal price { get; set; }
        public int stockQuantity { get; set; }
        public int categoryId { get; set; }
        public Category category { get; set; }
        public bool isDeleted { get; set; }
        public DateTime createdAt { get; set; } = DateTime.UtcNow;
        public DateTime? updatedAt { get; set; }
    }
}
