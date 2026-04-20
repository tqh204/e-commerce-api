using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Features.Review.Queries
{
    public record ReviewDTO
    (
        Guid ReviewId, 
        string? Username,
        int Rating,
        string? Comment,
        DateTime CreatedAt 
    );

    public class ReviewPagedResult
    {
        public double AverageRating { get; set; }     
        public int TotalReviews { get; set; }          
        public int Page { get; set; }
        public int Size { get; set; }
        public int TotalPages { get; set; }
        public IReadOnlyList<ReviewDTO> Items { get; set; } = new List<ReviewDTO>();
    }
}
