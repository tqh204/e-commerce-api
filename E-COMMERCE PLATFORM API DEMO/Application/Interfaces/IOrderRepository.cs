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
    }
}
