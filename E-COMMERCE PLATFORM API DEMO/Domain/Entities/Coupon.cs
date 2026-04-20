using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entities
{
    public class Coupon
    {
        public Guid couponId { get; set; }
        public string? code {  get; set; }
        public DiscountType discountType { get; set; }
        public decimal value { get; set; }
        public decimal minOrderValue { get; set; }
        public DateTime startDate { get; set; }
        public DateTime endDate { get; set; }
        public int usageLimit { get; set; }
        public int usedCount { get; set; }
        public bool isActive { get; set; }
        public DateTime createdAt { get; set; } = DateTime.UtcNow;
        public DateTime? updatedAt { get; set; }
    }
}
