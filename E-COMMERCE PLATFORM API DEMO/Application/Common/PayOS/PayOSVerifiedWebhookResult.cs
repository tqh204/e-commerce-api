using System;

namespace Application.Common.PayOS
{
    public record PayOSVerifiedWebhookResult(
        long orderCode,
        string? paymentLinkId,
        string? providerReference,
        DateTime? transactionDateTime
    );
}
