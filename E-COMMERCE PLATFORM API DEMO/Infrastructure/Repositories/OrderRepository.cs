using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly AppDBContext _context;
        public OrderRepository(AppDBContext context) => _context = context;
        public async Task AddAsync(Order order) => await _context.Orders.AddAsync(order);


        public async Task<IReadOnlyList<Order>> GetOrderByUserId(Guid userId) => await _context.Orders.Include(x => x.items)
                                                                                                      .Where(u => u.userId == userId)
                                                                                                      .OrderByDescending(u => u.createdAt)
                                                                                                      .ToListAsync();
    }
}
