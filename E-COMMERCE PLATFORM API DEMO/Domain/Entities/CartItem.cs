using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entities
{
    public class CartItem
    {
        public Guid cartItemId {  get; set; }
        public Guid cartId { get; set; }
        public Guid productId { get; set; }
        public int quantity { get; set; }
        public decimal unitPrice { get; set; }
        public DateTime createdAt { get; set; } = DateTime.UtcNow;
        public DateTime? updatedAt { get; set; }
        public Cart? cart { get; set; }
        public Product? product { get; set; }
    }
}
