using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entities
{
    public class OrderItem
    {
        public Guid orderItemId {  get; set; }
        public Guid orderId { get; set; }
        public Guid productId { get; set; }
        public string? productName { get; set; }
        public int quantity { get; set; }
        public decimal unitPrice { get; set; }  
        public decimal lineTotal { get; set; }

        public Order? order { get; set; }
        public Product? product { get; set; }
    }
}
