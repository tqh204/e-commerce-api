using Domain.Entities;
using System;

namespace Application.Interfaces
{
    public interface IPaymentRepository
    {
        Task AddAsync(Payment payment);
        Task<Payment?> GetByPaymentIdAsync(Guid paymentId);
        Task<Payment?> GetByOrderCodeAsync(long orderCode);
        void Update(Payment payment);
    }
}
