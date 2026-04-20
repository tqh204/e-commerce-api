using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entities
{
    public class Review
    {
        public Guid reviewId { get; set; }
        public Guid userId {  get; set; }
        public Guid productId { get; set; } 
        public int rating { get; set; }
        public string? comment { get; set; }
        public DateTime createdAt {  get; set; }

        public User? user {  get; set; }
        public Product? product { get; set; }
    }
}
