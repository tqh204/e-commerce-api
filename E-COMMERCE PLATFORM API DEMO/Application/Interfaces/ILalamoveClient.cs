using Application.Common.Lalamove;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces
{
    public interface ILalamoveClient
    {
        Task<ShippingFeeResponse> GetQuotationAsync(
            GetShippingFeeRequest request,
            CancellationToken cancellationToken = default);

        Task<LalamoveCreateOrderResult> CreateOrderAsync(
            CreateShipmentRequest request,
            string pickupStopId,
            string dropoffStopId,
            CancellationToken cancellationToken = default);

        Task<LalamoveOrderDetailResult> GetOrderDetailAsync(
            string providerOrderId,
            CancellationToken cancellationToken = default);
    }
}
