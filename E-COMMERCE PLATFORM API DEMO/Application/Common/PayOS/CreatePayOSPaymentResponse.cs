using System;

namespace Application.Common.PayOS
{
    public record CreatePayOSPaymentResponse(
        Guid paymentId,
        long orderCode,
        string checkoutUrl,
        string status
    );
}
