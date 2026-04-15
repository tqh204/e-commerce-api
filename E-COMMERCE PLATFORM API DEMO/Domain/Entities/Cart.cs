using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entities
{
    public class Cart
    {
        public Guid cartId { get; set; }
        public Guid userId { get; set; }
        public DateTime createdAt { get; set; } = DateTime.UtcNow;
        public DateTime? updatedAt { get; set; }
        public User? user { get; set; }
        public ICollection<CartItem> items { get; set; } = new List<CartItem>();
    }
}
