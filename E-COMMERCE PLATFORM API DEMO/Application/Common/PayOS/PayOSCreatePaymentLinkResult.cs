using System;

namespace Application.Common.PayOS
{
    public record PayOSCreatePaymentLinkResult(
        string paymentLinkId,
        string checkoutUrl,
        string providerStatus
    );
}
