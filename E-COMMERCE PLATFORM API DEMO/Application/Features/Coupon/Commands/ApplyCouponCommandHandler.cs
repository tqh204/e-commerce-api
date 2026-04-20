using Application.Interfaces;
using Domain.Entities;
using Domain.Enums;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Features.Coupon.Commands
{
    public class ApplyCouponCommandHandler : IRequestHandler<ApplyCouponCommand, Result<bool>>
    {
        private readonly ICartRepository _cartRepository;
        private readonly ICouponRepository _couponRepository;
        private readonly IUnitOfWork _unitOfWork;
        public ApplyCouponCommandHandler(ICartRepository cartRepository, ICouponRepository couponRepository, IUnitOfWork unitOfWork)
        {
            _cartRepository = cartRepository;
            _couponRepository = couponRepository;
            _unitOfWork = unitOfWork;
        }
        public async Task<Result<bool>> Handle(ApplyCouponCommand request, CancellationToken cancellationToken)
        {
            var cart = await _cartRepository.GetByUserIdAsync(request.userId);

            if(cart == null || !cart.items.Any())
            {
                return Result<bool>.Failure("Không tìm thấy cart hoặc giỏ hàng đang trống!");
            }

            var code = await _couponRepository.GetCodeAsync(request.code.Trim().ToUpper());
            if(code == null)
            {
                return Result<bool>.Failure("Không tìm thấy code");
            }
            if(!code.isActive)
            {
                return Result<bool>.Failure("Code không hoạt động!");
            }

            if(code.startDate > DateTime.UtcNow)
            {
                return Result<bool>.Failure("Code chưa khả dụng");
            }

            if(code.endDate < DateTime.UtcNow)
            {
                return Result<bool>.Failure("Code không còn khả dụng");
            }

            if(code.usedCount >= code.usageLimit)
            {
                return Result<bool>.Failure("Code đã hết lượt sử dụng");
            }

            var subTotal = cart.items.Sum(i => i.unitPrice * i.quantity);

            if(subTotal < code.minOrderValue)
            {
                return Result<bool>.Failure("Code chưa đạt đủ điều kiện để sử dụng");
            }
            decimal discountAmount;
            if (code.discountType == DiscountType.Percentage)
            {
                 discountAmount = subTotal * code.value / 100;
            }
            else if(code.discountType == DiscountType.FixedAmount)
            {
                 discountAmount = code.value;
            }
            else
            {
                return Result<bool>.Failure("Kiểu discount không hợp lệ");
            }

            if(discountAmount > subTotal)
            {
                discountAmount = subTotal;
            }

            cart.couponId = code.couponId;
            cart.couponCode = code.code;
            cart.discountAmount = discountAmount;
            cart.updatedAt = DateTime.UtcNow;

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result<bool>.Success(true);
        }

    }

}
