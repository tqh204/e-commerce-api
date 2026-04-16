using Application.Features.Order.Commands;
using Application.Features.Order.Queries;
using MediatR;
using System.Security.Claims;

namespace WebAPI.Endpoints
{
    public static class OrderEndpoint
    {
        public static void MapOrderEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/v1/orders").RequireAuthorization();

            group.MapPost("/checkout", async (
                ClaimsPrincipal user,
                IMediator mediator) =>
            {
                var userIdValue = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if(string.IsNullOrEmpty(userIdValue))
                {
                    return Results.Unauthorized();
                }

                var userId = Guid.Parse(userIdValue);

                var result = await mediator.Send(new CheckoutCommand(userId));

                if (!result.IsSuccess)
                {
                    return Results.BadRequest(new {Message = result.ErrorMessage});
                }

                return Results.Ok(new {Message = "Đặt hàng thành công", OrderId = result.Data});
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

                var result = await mediator.Send(new GetOrderQuery(userId));
                return Results.Ok(result);
            });
        }
    }
}
