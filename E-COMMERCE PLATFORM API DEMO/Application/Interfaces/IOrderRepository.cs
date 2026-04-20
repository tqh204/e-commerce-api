using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces
{
    public interface IOrderRepository
    {
        Task AddAsync(Order order);
        Task<IReadOnlyList<Order>> GetOrderByUserId(Guid userId);
        Task<Order?> GetOrderIdAsync(Guid orderId);
        void Update(Order order);
    }
}
