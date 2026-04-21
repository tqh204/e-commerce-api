using Application.Features.Variant.Commands;
using Application.Features.Variant.Queries;
using FluentValidation;
using MediatR;

namespace WebAPI.Endpoints
{
    public static class VariantEndpoint
    {
        public static void MapVariantEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/v1/admin/variants")
                           .RequireAuthorization("AdminOnly");

            group.MapPost("/", async (
                CreateVariantCommand command,
                IMediator mediator,
                IValidator<CreateVariantCommand> validator) =>
            {
                var validationResult = await validator.ValidateAsync(command);
                if (!validationResult.IsValid)
                    return Results.ValidationProblem(validationResult.ToDictionary());

                var result = await mediator.Send(command);
                if (!result.IsSuccess) return Results.BadRequest(new { Message = result.ErrorMessage });

                return Results.Ok(new { Message = "Tạo biến thể thành công", variantId = result.Data });
            });

            group.MapPut("/{variantId:guid}", async (
                        Guid variantId,
                        UpdateVariantCommand command,
                        IMediator mediator,
                        IValidator<UpdateVariantCommand> validator) =>
            {
                var updateCommand = command with { variantId = variantId };

                var validationResult = await validator.ValidateAsync(updateCommand);
                if (!validationResult.IsValid)
                    return Results.ValidationProblem(validationResult.ToDictionary());

                var result = await mediator.Send(updateCommand);
                if (!result.IsSuccess)
                    return Results.BadRequest(new { Message = result.ErrorMessage });

                return Results.Ok(new { Message = "Cập nhật biến thể thành công." });
            });

            group.MapDelete("/{variantId:guid}", async (
                            Guid variantId,
                            IMediator mediator,
                            IValidator<DeleteVariantCommand> validator) =>
            {
                var command = new DeleteVariantCommand(variantId);

                var validationResult = await validator.ValidateAsync(command);
                if (!validationResult.IsValid)
                    return Results.ValidationProblem(validationResult.ToDictionary());

                var result = await mediator.Send(command);
                if (!result.IsSuccess)
                    return Results.NotFound(new { Message = result.ErrorMessage });

                return Results.Ok(new { Message = "Xóa biến thể thành công." });
            });

            group.MapGet("/product/{productId:guid}", async (
                            Guid productId,
                            int page,
                            int size,
                            IMediator mediator,
                            IValidator<GetProductVariantsQuery> validator) =>
            {
                var query = new GetProductVariantsQuery(productId, page <= 0 ? 1 : page, size <= 0 ? 10 : size);
                var validationResult = await validator.ValidateAsync(query);
                if (!validationResult.IsValid)
                    return Results.ValidationProblem(validationResult.ToDictionary());
                var result = await mediator.Send(query);
                return Results.Ok(result);
            });
        }
    }
}