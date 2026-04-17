using Application.Features.Coupon.Commands;
using FluentValidation;
using MediatR;

namespace WebAPI.Endpoints
{
    public static class CouponEndpoint
    {
        public static void MapCouponEndpoints(this IEndpointRouteBuilder app)
        {
            //var group = app.MapGroup("/api/v1");

            app.MapPost("/api/v1/admin/coupons", async (
                CreateCouponCommand command,
                IMediator mediator,
                IValidator<CreateCouponCommand> validator) =>
                {
                    var validationResult = await validator.ValidateAsync(command);
                    if(!validationResult.IsValid)
                    {
                    return Results.ValidationProblem(validationResult.ToDictionary());
                    }

                    var result = await mediator.Send(command);
                    if(!result.IsSuccess)
                    {
                        return Results.BadRequest(new { Message = result.ErrorMessage });
                    }
                    return Results.Ok(new {couponId = result.Data, Message = "Tạo coupon thành công!"});

                }).RequireAuthorization("AdminOnly");
        }
    }
}
