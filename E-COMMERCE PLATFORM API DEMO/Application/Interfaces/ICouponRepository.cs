using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces
{
    public interface ICouponRepository
    {
        Task<Coupon?> GetCouponByIdAsync(Guid couponId);
        Task<Coupon?> GetCodeAsync(string code);
        Task AddAsync(Coupon coupon);
        Task<bool> ExistsByCodeAsync(string code);
        void Update(Coupon coupon);
    }
}
