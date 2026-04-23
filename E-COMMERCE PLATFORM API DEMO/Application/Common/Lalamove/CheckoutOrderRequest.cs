namespace Application.Common.Lalamove
{
    public record CheckoutOrderRequest(
        CreateShipmentRequest? shipment = null
    );
}
