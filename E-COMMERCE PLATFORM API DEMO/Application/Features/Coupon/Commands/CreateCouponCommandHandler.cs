using System;
using System.Collections.Generic;
using System.Text;
using Application.Interfaces;
using Domain.Entities;
using MediatR;
using Application.Common.Results;
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
                return Result<Guid>.Failure("code dã t?n t?i");
            }

            //var discount = request.discountType.Trim().ToUpper();
            //if(discount != "PERCENTAGE" && discount != "FIXED_AMOUNT")
            //{
            //    return Result<Guid>.Failure("Ki?u discount không h?p l?");
            //}

            if(request.value <= 0)
            {
                return Result<Guid>.Failure("Value không du?c là 0 ho?c âm");
            }

            if(request.usageLimit <= 0)
            {
                return Result<Guid>.Failure("Hãy ghi s? l?n s? d?ng h?p l?");
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

