using Application.Features.Cart.Commands;
using Application.Features.Cart.Queries;
using Azure.Core;
using Domain.Entities;
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

            group.MapGet("/", async (
                ClaimsPrincipal user,
                IMediator mediator) =>
            {
                var userIdValue = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if(string.IsNullOrEmpty(userIdValue))
                {
                    return Results.Unauthorized();
                }    

                var userId = Guid.Parse(userIdValue);

                var result = await mediator.Send(new GetCartQuery(userId));

                if(!result.IsSuccess)
                {
                    return Results.BadRequest(new {Message = result.ErrorMessage});
                }
                return Results.Ok(result.Data);   
            });

            group.MapPut("/items/{cartItemId:guid}", async (
                Guid cartItemId,
                UpdateCartItemRequest request,
                ClaimsPrincipal user,
                IMediator mediator,
                IValidator<UpdateCartItemCommand> validator) =>
            {
                var userIdValue = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(userIdValue))
                {
                    return Results.Unauthorized();
                }

                var userId = Guid.Parse(userIdValue);

                var command = new UpdateCartItemCommand(
                    userId,
                    cartItemId,
                    request.quantity);

                var validationResult = await validator.ValidateAsync(command);

                if(!validationResult.IsValid)
                {
                    return Results.ValidationProblem(validationResult.ToDictionary());
                }

                var result = await mediator.Send(command);
                if (!result.IsSuccess)
                {
                    return Results.BadRequest(new { Message = result.ErrorMessage });
                }
                return Results.Ok(new {Message = "Cập nhật giỏ hàng thành cong!"});
            });

        }
    }
    public record AddToCartRequest(Guid productId, int quantity);
    public record UpdateCartItemRequest(int quantity);
}
