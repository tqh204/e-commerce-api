using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Repositories
{
    public class CouponRepository : ICouponRepository
    {
        private readonly AppDBContext _context;
        public CouponRepository(AppDBContext context) => _context = context;

        public async Task AddAsync(Coupon coupon) => await _context.AddAsync(coupon);

        public async Task<bool> ExistsByCodeAsync(string code) => await _context.Coupons.AnyAsync(u => u.code == code);

        public async Task<Coupon?> GetCodeAsync(string code) => await _context.Coupons.FirstOrDefaultAsync(u => u.code == code);

        public async Task<Coupon?> GetCouponByIdAsync(Guid couponId) => await _context.Coupons.FirstOrDefaultAsync(u => u.couponId == couponId);

        public void Update(Coupon coupon) => _context.Coupons.Update(coupon);
    }
}
