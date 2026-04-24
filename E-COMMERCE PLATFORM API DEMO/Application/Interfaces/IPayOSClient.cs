using Application.Common.PayOS;
using Application.Common.Results;
using Domain.Entities;

namespace Application.Interfaces
{
    public interface IPayOSClient
    {
        Task<Result<PayOSCreatePaymentLinkResult>> CreatePaymentLinkAsync(
            Payment payment,
            CheckoutSnapshot snapshot,
            User user,
            CancellationToken cancellationToken = default);

        Task<Result<PayOSPaymentLinkStatusResult>> GetPaymentStatusAsync(
            long orderCode,
            CancellationToken cancellationToken = default);

        Task<Result<PayOSVerifiedWebhookResult>> VerifyWebhookAsync(
            string rawPayload,
            CancellationToken cancellationToken = default);
    }
}
