using Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Features.Coupon.Commands
{
    public record ApplyCouponCommand(Guid userId, string code) : IRequest<Result<bool>>;
}
