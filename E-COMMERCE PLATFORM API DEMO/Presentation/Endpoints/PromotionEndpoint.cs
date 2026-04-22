using Application.Features.Promotion.Commands;
using FluentValidation;
using MediatR;

namespace WebAPI.Endpoints
{
    public static class PromotionEndpoint
    {
        public static void MapPromotionEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/v1/admin/promotions/rules")
                .RequireAuthorization("AdminOnly");

            group.MapPost("/", async (
                CreatePromotionRuleCommand command,
                IMediator mediator
                // IValidator<CreatePromotionRuleCommand> validator
                ) =>
            {
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

                return Results.Ok(new { ruleId = result.Data, Message = "Tạo promotion rule thành công!" });
            });

            group.MapPut("/{ruleId:guid}", async (
                Guid ruleId,
                UpdatePromotionRuleCommand command,
                IMediator mediator
                // IValidator<UpdatePromotionRuleCommand> validator
                ) =>
            {
                var updateCommand = command with { ruleId = ruleId };
                // var validationResult = await validator.ValidateAsync(updateCommand);
                // if (!validationResult.IsValid)
                // {
                //     return Results.ValidationProblem(validationResult.ToDictionary());
                // }

                var result = await mediator.Send(updateCommand);
                if (!result.IsSuccess)
                {
                    return Results.BadRequest(new { Message = result.ErrorMessage });
                }

                return Results.Ok(new { Message = "Cập nhật promotion rule thành công!" });
            });

            group.MapDelete("/{ruleId:guid}", async (
                Guid ruleId,
                IMediator mediator
                // IValidator<DeletePromotionRuleCommand> validator
                ) =>
            {
                var command = new DeletePromotionRuleCommand(ruleId);
                // var validationResult = await validator.ValidateAsync(command);
                // if (!validationResult.IsValid)
                // {
                //     return Results.ValidationProblem(validationResult.ToDictionary());
                // }

                var result = await mediator.Send(command);
                if (!result.IsSuccess)
                {
                    return Results.NotFound(new { Message = result.ErrorMessage });
                }

                return Results.Ok(new { Message = "Xóa promotion rule thành công!" });
            });
        }
    }
}
