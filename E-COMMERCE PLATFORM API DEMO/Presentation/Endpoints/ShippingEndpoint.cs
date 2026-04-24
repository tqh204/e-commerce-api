using Application.Common.Lalamove;
using Application.Interfaces;

namespace WebAPI.Endpoints
{
    public static class ShippingEndpoint
    {
        public static void MapShippingEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/v1/shipping").RequireAuthorization();

            app.MapGet("/api/v1/shipping/lalamove/webhook", () =>
            {
                return Results.Ok(new { Message = "Webhook endpoint is reachable" });
            });

            group.MapPost("/fee", async (
                GetShippingFeeRequest request,
                IShipmentService shipmentService) =>
            {
                var result = await shipmentService.GetShippingFeeAsync(request);

                if (!result.IsSuccess)
                {
                    return Results.BadRequest(new { Message = result.ErrorMessage });
                }

                return Results.Ok(result.Data);
            });

            group.MapGet("/lalamove/orders/{providerOrderId}", async (
                string providerOrderId,
                IShipmentService shipmentService) =>
            {
                var result = await shipmentService.GetOrderDetailAsync(providerOrderId);

                if (!result.IsSuccess)
                {
                    return Results.BadRequest(new { Message = result.ErrorMessage });
                }

                return Results.Ok(result.Data);
            });

            app.MapPost("/api/v1/shipping/lalamove/webhook", async (
                HttpRequest request,
                IShipmentService shipmentService) =>
            {
                using var reader = new StreamReader(request.Body);
                var rawPayload = await reader.ReadToEndAsync();

                var result = await shipmentService.HandleWebhookAsync(rawPayload);

                if (!result.IsSuccess)
                {
                    return Results.BadRequest(new { Message = result.ErrorMessage });
                }

                return Results.Ok(new { Message = "Webhook processed" });
            });
        }
    }
}
