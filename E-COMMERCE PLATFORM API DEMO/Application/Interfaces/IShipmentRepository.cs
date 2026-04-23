using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces
{
    public interface IShipmentRepository
    {
        Task AddAsync(Shipment shipment);
        Task<Shipment?> GetByOrderIdAsync(Guid orderId);
        Task<Shipment?> GetByProviderOrderIdAsync(string providerOrderId);
        void Update(Shipment shipment);
    }
}
