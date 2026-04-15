using Application.Features.Cart.Commands;
using Azure.Core;
using FluentValidation;
using MediatR;
using System.Security.Claims;

namespace WebAPI.Endpoints
{
    public static class CartEndpoint
    {
        public static void MapCartEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/v1/cart").RequireAuthorization();

            group.MapPost("/add", async (
                AddToCartRequest request,
                ClaimsPrincipal user,
                IMediator mediator,
                IValidator<AddToCartCommand> validator) =>
            {
                var userIdValue = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if(string.IsNullOrEmpty(userIdValue))
                {
                    return Results.Unauthorized();
                }

                var userId = Guid.Parse(userIdValue);

                var command = new AddToCartCommand(
                    userId,
                    request.productId,
                    request.quantity);

                var validationResult = await validator.ValidateAsync(command);

                if(!validationResult.IsValid)
                {
                    return Results.ValidationProblem(validationResult.ToDictionary());
                }

                var result = await mediator.Send(command);

                if(!result.IsSuccess)
                {
                    return Results.BadRequest(new {Message = result.ErrorMessage});
                }
                return Results.Ok(new { Message = "Thêm sản phẩm vào giỏ hàng thành công!" });
            });
        }
    }
    public record AddToCartRequest(Guid productId, int quantity);
}
