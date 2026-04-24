using Application.Common.Lalamove;

namespace Application.Common.PayOS
{
    public record CreatePayOSPaymentRequest(
        string? description,
        string returnUrl,
        string cancelUrl,
        CreateShipmentRequest? shipment
    );
}
