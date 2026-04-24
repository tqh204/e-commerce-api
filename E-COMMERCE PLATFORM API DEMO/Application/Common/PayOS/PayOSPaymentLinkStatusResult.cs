using System;

namespace Application.Common.PayOS
{
    public record PayOSPaymentLinkStatusResult(
        string providerStatus,
        string? paymentLinkId,
        string? providerReference,
        DateTime? transactionDateTime
    );
}
