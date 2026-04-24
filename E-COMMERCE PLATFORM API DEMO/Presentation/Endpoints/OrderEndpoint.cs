using Application.Common.Lalamove;
using Application.Common.PayOS;
using Application.Features.Order.Commands;
using Application.Features.Order.Queries;
using Application.Interfaces;
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
                CheckoutOrderRequest? request,
                IPaymentService paymentService) =>
            {
                var userIdValue = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdValue))
                {
                    return Results.Unauthorized();
                }

                var userId = Guid.Parse(userIdValue);
                var paymentResult = await paymentService.CreatePaymentLinkAsync(
                    userId,
                    new CreatePayOSPaymentRequest(
                        request?.description,
                        request?.returnUrl ?? string.Empty,
                        request?.cancelUrl ?? string.Empty,
                        request?.shipment));

                if (!paymentResult.IsSuccess)
                {
                    return Results.BadRequest(new { Message = paymentResult.ErrorMessage });
                }

                return Results.Ok(new
                {
                    Message = "Tạo link thanh toán PayOS thành công",
                    PaymentId = paymentResult.Data!.paymentId,
                    OrderCode = paymentResult.Data.orderCode,
                    CheckoutUrl = paymentResult.Data.checkoutUrl,
                    Status = paymentResult.Data.status
                });
            });

            group.MapPost("/checkout/manual", async (
                ClaimsPrincipal user,
                CheckoutOrderRequest? request,
                IMediator mediator,
                IShipmentService shipmentService) =>
            {
                var userIdValue = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdValue))
                {
                    return Results.Unauthorized();
                }

                var userId = Guid.Parse(userIdValue);
                var result = await mediator.Send(new CheckoutCommand(userId));

                if (!result.IsSuccess)
                {
                    return Results.BadRequest(new { Message = result.ErrorMessage });
                }

                if (request?.shipment == null)
                {
                    return Results.Ok(new
                    {
                        Message = "Đặt hàng thành công",
                        OrderId = result.Data
                    });
                }

                var shipmentResult = await shipmentService.CreateShipmentAsync(result.Data, request.shipment);
                if (!shipmentResult.IsSuccess)
                {
                    return Results.Ok(new
                    {
                        Message = "Đặt hàng thành công nhưng tạo shipment thất bại",
                        OrderId = result.Data,
                        ShipmentCreated = false,
                        ShipmentError = shipmentResult.ErrorMessage
                    });
                }

                return Results.Ok(new
                {
                    Message = "Đặt hàng thành công và đã tạo shipment",
                    OrderId = result.Data,
                    ShipmentCreated = true,
                    ShipmentId = shipmentResult.Data.shipmentId,
                    ProviderOrderId = shipmentResult.Data.providerOrderId,
                    ProviderStatus = shipmentResult.Data.providerStatus
                });
            });

            group.MapGet("/", async (
                ClaimsPrincipal user,
                IMediator mediator) =>
            {
                var userIdValue = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdValue))
                {
                    return Results.Unauthorized();
                }

                var userId = Guid.Parse(userIdValue);
                var result = await mediator.Send(new GetOrderQuery(userId));
                return Results.Ok(result);
            });

            group.MapPut("/{orderId:guid}/complete", async (
                ClaimsPrincipal user,
                Guid orderId,
                IMediator mediator) =>
            {
                var userIdValue = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var userId = Guid.Parse(userIdValue!);
                var command = new CompleteOrderCommand(userId, orderId);
                var result = await mediator.Send(command);

                if (!result.IsSuccess)
                {
                    return Results.BadRequest(new { Message = result.ErrorMessage });
                }

                return Results.Ok(new { Message = "Order đã được cập nhật thành COMPLETED" });
            }).RequireAuthorization("AdminOnly");

            group.MapPost("/{orderId:guid}/create-shipment", async (
                Guid orderId,
                CreateShipmentRequest request,
                IShipmentService shipmentService) =>
            {
                var result = await shipmentService.CreateShipmentAsync(orderId, request);

                if (!result.IsSuccess)
                {
                    return Results.BadRequest(new { Message = result.ErrorMessage });
                }

                return Results.Ok(new
                {
                    Message = "Tạo shipment thành công",
                    ShipmentId = result.Data.shipmentId,
                    ProviderOrderId = result.Data.providerOrderId,
                    ProviderStatus = result.Data.providerStatus
                });
            }).RequireAuthorization();
        }
    }
}
