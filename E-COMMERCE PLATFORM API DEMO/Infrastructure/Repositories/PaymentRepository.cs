using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class PaymentRepository : IPaymentRepository
    {
        private readonly AppDBContext _context;

        public PaymentRepository(AppDBContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Payment payment) => await _context.Payments.AddAsync(payment);

        public async Task<Payment?> GetByPaymentIdAsync(Guid paymentId)
            => await _context.Payments.FirstOrDefaultAsync(x => x.paymentId == paymentId);

        public async Task<Payment?> GetByOrderCodeAsync(long orderCode)
            => await _context.Payments.FirstOrDefaultAsync(x => x.orderCode == orderCode);

        public void Update(Payment payment) => _context.Payments.Update(payment);
    }
}
