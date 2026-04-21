using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entities
{
    public class Variant
    {
        public Guid variantId { get; set; }
        public Guid productId { get; set; }
        public Product? product { get; set; }
        public string? sku { get; set; }
        public string? size{ get; set; }
        public string? color { get; set; }
        public string? material { get; set; }    
        public decimal price { get; set; }
        public int inventory { get; set; }
        public bool isDeleted { get; set; }
        public DateTime? createdAt { get; set; }
        public DateTime? updatedAt { get; set; }

    }
}
