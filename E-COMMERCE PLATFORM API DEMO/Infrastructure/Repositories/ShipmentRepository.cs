using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Repositories
{
    public class ShipmentRepository : IShipmentRepository
    {
        private readonly AppDBContext _context;

        public ShipmentRepository(AppDBContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Shipment shipment) => await _context.Shipments.AddAsync(shipment);

        public async Task<Shipment?> GetByOrderIdAsync(Guid orderId) => await _context.Shipments.FirstOrDefaultAsync(s => s.orderId == orderId);

        public async Task<Shipment?> GetByProviderOrderIdAsync(string providerOrderId) => await _context.Shipments.FirstOrDefaultAsync(s => s.providerOrderId == providerOrderId);

        public void Update(Shipment shipment) => _context.Shipments.Update(shipment);
    }
}
