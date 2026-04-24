namespace Application.Common.Lalamove
{
    public record CheckoutOrderRequest(
        string? description = null,
        string? returnUrl = null,
        string? cancelUrl = null,
        CreateShipmentRequest? shipment = null
    );
}
