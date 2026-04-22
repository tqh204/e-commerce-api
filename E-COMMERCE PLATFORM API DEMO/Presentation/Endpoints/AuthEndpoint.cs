using Application.Features.Auth.Commands.Login;
using Application.Features.Auth.Commands.Register;
using Domain.Entities;
using FluentValidation;
using MediatR;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
namespace Presentation.Endpoints
{
    public static class AuthEndpoint
    {
        public static void MapAuthEndpoints(this IEndpointRouteBuilder app) {
            var group = app.MapGroup("/api/v1/auth");

            group.MapPost("/register", async (
                RegisterUserCommand command,
                IMediator mediator
                //IValidator<RegisterUserCommand> validator
                ) =>
            {
                //Implement validate input
                //var validationResult = await validator.ValidateAsync(command);
                //if (!validationResult.IsValid)
                //{
                //    return Results.ValidationProblem(validationResult.ToDictionary());
                //}

                var userId = await mediator.Send(command);

                return Results.Ok(new { Message = "Đăng ký thành công!", UserId = userId });
            });

            group.MapPost("/login", async (
                LoginCommand command,
                IMediator mediator,
                IValidator<LoginCommand> validator,
                HttpContext httpContext) =>
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
                    var accessToken = result.Data!.AccessToken;
                    httpContext.Response.Cookies.Append("access_Token", accessToken, new CookieOptions
                    {
                        HttpOnly = true,
                        Secure = false,
                        SameSite = SameSiteMode.Lax,
                        Expires = DateTimeOffset.UtcNow.AddMinutes(60)
                    });

                    return Results.Ok(new { Message = "Đăng nhập thành công!", AccessToken = result.Data });


                });

            group.MapGet("/me", (ClaimsPrincipal user) =>
            {
                var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var email = user.FindFirst(ClaimTypes.Email)?.Value;
                var username = user.FindFirst(ClaimTypes.Name)?.Value;
                var role = user.FindFirst(ClaimTypes.Role)?.Value;

                return Results.Ok(new
                {
                    UserId = userId,
                    Email = email,
                    Username = username,
                    Role = role
                });
            }).RequireAuthorization();

            group.MapGet("/admin-only", () =>
            {
                return Results.Ok(new { Message = " Bạn là admin hỏ? Hợp lệ nho~~" });
            }).RequireAuthorization("AdminOnly");
        }
    }
}
