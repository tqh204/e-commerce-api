using System;
using System.Collections.Generic;
using System.Text;
using Application.Interfaces;
using Domain.Entities;
using MediatR;
namespace Application.Features.Coupon.Commands
{
    public class CreateCouponCommandHandler : IRequestHandler<CreateCouponCommand, Result<Guid>>
    {
        private ICouponRepository _couponRepository;
        private IUnitOfWork _unitOfWork;
        public CreateCouponCommandHandler(ICouponRepository couponRepository, IUnitOfWork unitOfWork)
        {
            _couponRepository = couponRepository;
            _unitOfWork = unitOfWork;
        }
        public async Task<Result<Guid>> Handle(CreateCouponCommand request, CancellationToken cancellationToken)
        {
            var normalizeCode = request.code.Trim().ToUpper();
            var couponCode = await _couponRepository.GetCodeAsync(normalizeCode);
            if(couponCode != null)
            {
                return Result<Guid>.Failure("code đã tồn tại");
            }

            //var discount = request.discountType.Trim().ToUpper();
            //if(discount != "PERCENTAGE" && discount != "FIXED_AMOUNT")
            //{
            //    return Result<Guid>.Failure("Kiểu discount không hợp lệ");
            //}

            if(request.value <= 0)
            {
                return Result<Guid>.Failure("Value không được là 0 hoặc âm");
            }

            if(request.usageLimit <= 0)
            {
                return Result<Guid>.Failure("Hãy ghi số lần sử dụng hợp lệ");
            }

            var coupon = new Domain.Entities.Coupon
            {
                couponId = Guid.NewGuid(),
                code = normalizeCode,
                discountType = request.discountType,
                value = request.value,
                usageLimit = request.usageLimit,
                minOrderValue = request.minOrderValue,
                startDate = request.startDate,
                endDate = request.endDate,
                usedCount = 0,
                isActive = true,
                createdAt = DateTime.UtcNow,
                updatedAt = null
            };

            await _couponRepository.AddAsync(coupon);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result<Guid>.Success(coupon.couponId);
        }
    }
}
