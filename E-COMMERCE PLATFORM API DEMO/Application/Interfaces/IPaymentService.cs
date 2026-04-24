using Application.Common.PayOS;
using Application.Common.Results;
using System;

namespace Application.Interfaces
{
    public interface IPaymentService
    {
        Task<Result<CreatePayOSPaymentResponse>> CreatePaymentLinkAsync(
            Guid userId,
            CreatePayOSPaymentRequest request,
            CancellationToken cancellationToken = default);

        Task<Result<PaymentStatusResponse>> GetPaymentStatusAsync(
            Guid userId,
            Guid paymentId,
            CancellationToken cancellationToken = default);

        Task<Result<bool>> HandleWebhookAsync(
            string rawPayload,
            CancellationToken cancellationToken = default);
    }
}
