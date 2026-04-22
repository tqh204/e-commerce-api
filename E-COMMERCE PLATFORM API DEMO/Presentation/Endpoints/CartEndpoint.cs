using Application.Features.Cart.Commands;
using Application.Features.Cart.Queries;
using Application.Features.Coupon.Commands;
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
                IMediator mediator
                // IValidator<AddToCartCommand> validator
                ) =>
            {
                var userIdValue = user.FindFirst(ClaimTypes.NameIdentifier)?.Value; //Take the userId from accessToken when user login

                if(string.IsNullOrEmpty(userIdValue)) //If userIdValue has not any token, return Unautho
                {
                    return Results.Unauthorized();
                }

                var userId = Guid.Parse(userIdValue);//Parse String to Guid

                var command = new AddToCartCommand( //Create a request ticket
                    userId, //Get from JWT 
                    request.productId, //Get request from body
                    request.quantity);//same above

                // var validationResult = await validator.ValidateAsync(command); //Checking the command whether its valid?

                // if(!validationResult.IsValid)//If the result is not valid => Cannot get through Validator
                // {
                //     return Results.ValidationProblem(validationResult.ToDictionary());
                // }

                var result = await mediator.Send(command);//If the result is valid and get through validation, throw to mediator and the mediator to use the handle to resolve it.

                if(!result.IsSuccess)//If the result return back is not success, ==> bad request
                {
                    return Results.BadRequest(new {Message = result.ErrorMessage});
                }
                return Results.Ok(new { Message = "Thêm sản phẩm vào giỏ hàng thành công!" });//If success => ok
            });

            group.MapGet("/", async (
                ClaimsPrincipal user,
                IMediator mediator) =>
            {
                var userIdValue = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;//Take the userId from accessToken when user login   

                if (string.IsNullOrEmpty(userIdValue))
                {
                    return Results.Unauthorized();
                }    

                var userId = Guid.Parse(userIdValue);//parese string to Guid
                var query = new GetCartQuery(userId);//create a query and send to handler
                var result = await mediator.Send(query);

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
                IMediator mediator
                // IValidator<UpdateCartItemCommand> validator
                ) =>
            {
                var userIdValue = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(userIdValue))
                {
                    return Results.Unauthorized();
                }

                var userId = Guid.Parse(userIdValue);

                var command = new UpdateCartItemCommand(//create a command with userId is taken from token, cartItemId from route, quantity from request(body)
                    userId,
                    cartItemId,
                    request.quantity);

                // var validationResult = await validator.ValidateAsync(command);//checking command is vaid?

                // if(!validationResult.IsValid)
                // {
                //     return Results.ValidationProblem(validationResult.ToDictionary());
                // }

                var result = await mediator.Send(command);//if command is valid, put it into media to send handler
                if (!result.IsSuccess)
                {
                    return Results.BadRequest(new { Message = result.ErrorMessage });
                }
                return Results.Ok(new {Message = "Cập nhật giỏ hàng thành cong!"});
            });

            group.MapPost("/apply-coupon", async (
                RequestApplyCode request,
                ClaimsPrincipal user,
                IMediator mediator
                // IValidator<ApplyCouponCommand> validator
                ) =>
            {
                var userIdValue = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdValue))
                {
                    return Results.Unauthorized();
                }

                var userId = Guid.Parse(userIdValue);
                var command = new ApplyCouponCommand(
                    userId,
                    request.code);
                // var validationResult = await validator.ValidateAsync(command);
                // if (!validationResult.IsValid)
                // {
                //     return Results.ValidationProblem(validationResult.ToDictionary());
                // }

                var result = await mediator.Send(command);
                if (!result.IsSuccess)
                {
                    return Results.BadRequest(new { Message = result.ErrorMessage });
                }

                return Results.Ok(new { Message = "Áp dụng code thành công", IsApplied = result.Data });
            });

        }
    }
    //Why need this method? So Instead of using AddToCartCommand(That means you have to use the userId like a request also) Ex: U must sign to 3 parameters: userId, productId and quantity).
    //Its not safe for the user, cuz they can use another userId to sign into the request instead of their userId. 
    //In Conclusion, this method help user take userId directly from JWT and dont need to sign, all things user need to do is completed the productId and quantity
    //Or u could understand in another way like: AddToCartCommand is everything that the system needs to process in it AND AddToCartRequest is everything that the user(client) have permission to send a request
    public record AddToCartRequest(Guid productId, int quantity);
    
    
    public record UpdateCartItemRequest(int quantity);

    public record RequestApplyCode(string code);
}
