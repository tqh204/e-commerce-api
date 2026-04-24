using Application.Common.PayOS;
using Application.Interfaces;
using System.Security.Claims;
using WebAPI.Endpoints;

namespace Presentation.Endpoints
{
    public static class PaymentEndpoint
    {
        public static void MapPaymentEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/v1/payment/payos");

            group.MapPost("/create", async (
                ClaimsPrincipal user,
                CreatePayOSPaymentRequest request,
                IPaymentService paymentService) =>
            {
                var userIdValue = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrWhiteSpace(userIdValue))
                {
                    return Results.Unauthorized();
                }

                var result = await paymentService.CreatePaymentLinkAsync(Guid.Parse(userIdValue), request);
                if (!result.IsSuccess)
                {
                    return Results.BadRequest(new { Message = result.ErrorMessage });
                }

                return Results.Ok(result.Data);
            }).RequireAuthorization();

            group.MapPost("/webhook", async (
                HttpRequest httpRequest,
                IPaymentService paymentService) =>
            {
                using var reader = new StreamReader(httpRequest.Body);
                var rawPayload = await reader.ReadToEndAsync();
                var result = await paymentService.HandleWebhookAsync(rawPayload);

                if (!result.IsSuccess)
                {
                    return Results.BadRequest(new { Message = result.ErrorMessage });
                }

                return Results.Ok(new { Message = "Webhook PayOS processed" });
            });

            group.MapGet("/{paymentId:guid}", async (
                ClaimsPrincipal user,
                Guid paymentId,
                IPaymentService paymentService) =>
            {
                var userIdValue = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrWhiteSpace(userIdValue))
                {
                    return Results.Unauthorized();
                }

                var result = await paymentService.GetPaymentStatusAsync(Guid.Parse(userIdValue), paymentId);
                if (!result.IsSuccess)
                {
                    return Results.NotFound(new { Message = result.ErrorMessage });
                }

                return Results.Ok(result.Data);
            }).RequireAuthorization();
        }
    }
}
