using Application.Common.Lalamove;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces
{
    public interface IShipmentService
    {
        Task<Result<ShippingFeeResponse>> GetShippingFeeAsync(
            GetShippingFeeRequest request,
            CancellationToken cancellationToken = default);

        Task<Result<CreateShipmentResult>> CreateShipmentAsync(
            Guid orderId,
            CreateShipmentRequest request,
            CancellationToken cancellationToken = default);

        Task<Result<LalamoveOrderDetailResult>> GetOrderDetailAsync(
            string providerOrderId,
            CancellationToken cancellationToken = default);

        Task<Result<bool>> HandleWebhookAsync(
            string rawPayload,
            CancellationToken cancellationToken = default);
    }
}
