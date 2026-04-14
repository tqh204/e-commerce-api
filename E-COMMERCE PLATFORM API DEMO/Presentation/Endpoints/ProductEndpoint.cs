using Application.Features.Product.Commands;
using Application.Features.Product.Queries.GetProductByIdQuery;
using Application.Features.Product.Queries.GetProductQuery;
using Domain.Entities;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Components.Forms;
using System.ComponentModel.DataAnnotations;

namespace WebAPI.Endpoints
{
    public static class ProductEndpoint
    {
        public static async Task MapProductEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/v1/products");
            group.MapGet("/", async (
                IMediator mediator,
                IValidator < GetProductQuery > validator,
                int page = 1,
                int size = 10,
                int? categoryId = null,
                decimal? minPrice = null,
                decimal? maxPrice = null) =>
                {
                    var query = new GetProductQuery(page, size, categoryId, minPrice, maxPrice);

                    var validationResult = await validator.ValidateAsync(query);
                    if (!validationResult.IsValid)
                    {
                        return Results.ValidationProblem(validationResult.ToDictionary());
                    }
                    var result = await mediator.Send(query);
                    return Results.Ok(result);
                });

            group.MapGet("/{productId:guid}", async (
                IMediator mediator,
                IValidator<GetProductByIdQuery> Validator,
                Guid productId) =>
            {
                var query = new GetProductByIdQuery(productId);

                var validationResult = await Validator.ValidateAsync(query);
                if (!validationResult.IsValid)
                {
                    return Results.ValidationProblem(validationResult.ToDictionary());
                }
                var result = await mediator.Send(query);

                if (!result.IsSuccess)
                {
                    return Results.NotFound(new {Message = result.ErrorMessage});
                }
                return Results.Ok(result.Data);
            });

            group.MapPost("/", async (
                CreateProductCommand command,
                IMediator mediator,
                IValidator<CreateProductCommand> validator) =>
            {
                var validationResult = await validator.ValidateAsync(command);
                if(!validationResult.IsValid)
                {
                    return Results.ValidationProblem(validationResult.ToDictionary());
                }

                var result = mediator.Send(command);

                return Results.Ok(new { Message = "Đăng ký thành công!", product = result });
            });
                
        }
    }
}