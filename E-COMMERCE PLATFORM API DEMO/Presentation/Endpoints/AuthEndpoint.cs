using Application.Features.Auth.Commands.Login;
using Application.Features.Auth.Commands.Register;
using Domain.Entities;
using FluentValidation;
using MediatR;

namespace Presentation.Endpoints
{
    public static class AuthEndpoint
    {
        public static void MapAuthEndpoints(this IEndpointRouteBuilder app) {
            var group = app.MapGroup("/api/v1/auth");

            group.MapPost("/register", async (
                RegisterUserCommand command,
                IMediator mediator,
                IValidator<RegisterUserCommand> validator) =>
            {
                //Implement validate input
                var validationResult = await validator.ValidateAsync(command);
                if (!validationResult.IsValid)
                {
                    return Results.ValidationProblem(validationResult.ToDictionary());
                }

                var userId = await mediator.Send(command);

                return Results.Ok(new { Message = "Đăng ký thành công!", UserId = userId });
            });

            group.MapPost("/login", async (
                LoginCommand command,
                IMediator mediator,
                IValidator<LoginCommand> validator) =>
                {
                    //Implement validate input
                    var validationResult = await validator.ValidateAsync(command);
                    if (!validationResult.IsValid)
                    {
                        return Results.ValidationProblem(validationResult.ToDictionary());
                    }

                    var result = await mediator.Send(command);

                    if (!result.IsSuccess)
                    {
                        return Results.BadRequest(new { Message = result.ErrorMessage });
                    }

                    return Results.Ok(new { Message = "Đăng nhập thành công!", AccessToken = result.Data });


                });
        }
    }
}
