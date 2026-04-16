using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entities
{
    public class Order
    {   
        public Guid orderId {  get; set; }
        public Guid userId { get; set; }
        public decimal totalAmount { get; set; }
        public string status { get; set; } = "PENDING";
        public DateTime createdAt { get; set; } = DateTime.UtcNow;
        public DateTime? updatedAt { get; set; }

        public User? user { get; set; }
        public ICollection<OrderItem> items { get; set; } = new List<OrderItem>();

    }
}
