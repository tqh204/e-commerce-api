using Domain.Entities;

namespace Application.Interfaces
{
    public interface IShipmentQuotationRepository
    {
        Task AddAsync(ShipmentQuotation shipmentQuotation);
        Task<ShipmentQuotation?> GetByQuotationIdAsync(string quotationId);
    }
}
