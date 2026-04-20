using Application.Features.Review.Commands;
using Application.Features.Review.Queries;
using MediatR;
using System.Security.Claims;

namespace WebAPI.Endpoints
{
    public static class ReviewEndpoint
    {
        public static void MapReviewEndpoints(this IEndpointRouteBuilder app)
        {
            // POST /api/v1/reviews — Đăng review (cần đăng nhập)
            app.MapPost("/api/v1/reviews", async (
                ClaimsPrincipal user,
                PostReviewRequest request,
                IMediator mediator) =>
            {
                var userIdValue = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdValue))
                    return Results.Unauthorized();
                var userId = Guid.Parse(userIdValue);
                var command = new CreateReviewCommand(
                    userId,
                    request.ProductId,
                    request.Rating,
                    request.Comment);
                var result = await mediator.Send(command);
                if (!result.IsSuccess)
                    return Results.BadRequest(new { Message = result.ErrorMessage });
                return Results.Ok(new { Message = "Đánh giá thành công", ReviewId = result.Data });
            }).RequireAuthorization();
            // GET /api/v1/products/{id}/reviews — Xem reviews (public)
            app.MapGet("/api/v1/products/{id:guid}/reviews", async (
                Guid id,
                int page,
                int size,
                IMediator mediator) =>
            {
                // Mặc định page=1, size=10 nếu không truyền
                if (page <= 0) page = 1;
                if (size <= 0) size = 10;
                var query = new GetProductReviewsQuery(id, page, size);
                var result = await mediator.Send(query);
                return Results.Ok(result);
            });
        }
        // Request body cho POST
        public record PostReviewRequest(
            Guid ProductId,
            int Rating,
            string? Comment);
    }
}
