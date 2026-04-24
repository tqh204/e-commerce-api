using System;

namespace Application.Common.PayOS
{
    public record PaymentStatusResponse(
        Guid paymentId,
        string status,
        long orderCode,
        Guid? linkedOrderId,
        Guid? linkedShipmentId,
        string? checkoutUrl
    );
}
