using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class ShipmentQuotationRepository : IShipmentQuotationRepository
    {
        private readonly AppDBContext _context;

        public ShipmentQuotationRepository(AppDBContext context)
        {
            _context = context;
        }

        public async Task AddAsync(ShipmentQuotation shipmentQuotation)
            => await _context.ShipmentQuotations.AddAsync(shipmentQuotation);

        public async Task<ShipmentQuotation?> GetByQuotationIdAsync(string quotationId)
            => await _context.ShipmentQuotations.FirstOrDefaultAsync(x => x.quotationId == quotationId);
    }
}
