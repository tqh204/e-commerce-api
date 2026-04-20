using Application.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.Review.Queries
{
    public class GetProductReviewsQueryHandler : IRequestHandler<GetProductReviewsQuery, ReviewPagedResult>
    {
        private readonly IReviewRepository _reviewRepository;
        public GetProductReviewsQueryHandler(IReviewRepository reviewRepository)
        {
            _reviewRepository = reviewRepository;
        }
        public async Task<ReviewPagedResult> Handle(
            GetProductReviewsQuery request, CancellationToken cancellationToken)
        {
            var (items, totalCount) = await _reviewRepository
                .GetByProductIdAsync(request.ProductId, request.Page, request.Size);
            // Map entity → DTO
            var dtos = items.Select(r => new ReviewDTO(
                r.reviewId,
                r.user?.username ?? "Ẩn danh",
                r.rating,
                r.comment,
                r.createdAt
            )).ToList();
            // Tính avg rating từ tất cả review (totalCount items trong trang)
            double avgRating = dtos.Count > 0
                ? Math.Round(dtos.Average(r => r.Rating), 1)
                : 0;
            // ⚠️ Lưu ý: avg trên chỉ tính trong trang hiện tại.
            // Nếu muốn avg toàn bộ, cần thêm method riêng trong repository.
            // Xem gợi ý bên dưới ở phần nâng cao.
            return new ReviewPagedResult
            {
                AverageRating = avgRating,
                TotalReviews = totalCount,
                Page = request.Page,
                Size = request.Size,
                TotalPages = (int)Math.Ceiling((double)totalCount / request.Size),
                Items = dtos
            };
        }
    }
}

