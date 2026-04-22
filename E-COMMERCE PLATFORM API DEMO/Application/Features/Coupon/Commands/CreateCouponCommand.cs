using Domain.Entities;
using Domain.Enums;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using Application.Common.Results;

namespace Application.Features.Coupon.Commands
{
    public record CreateCouponCommand(
        string code,
        DiscountType discountType,
        decimal value,
        decimal minOrderValue,
        DateTime startDate,
        DateTime endDate,
        int usageLimit
        ) : IRequest<Result<Guid>>;
}

