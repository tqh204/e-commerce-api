using Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Features.Coupon.Commands
{
    public record CreateCouponCommand(
        string code,
        string discountType,
        decimal value,
        decimal minOrderValue,
        DateTime startDate,
        DateTime endDate,
        int usageLimit
        ) : IRequest<Result<Guid>>;
}
