using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Features.Coupon.Commands
{
    public class ApplyCouponCommandValidator : AbstractValidator<ApplyCouponCommand>
    {
        public ApplyCouponCommandValidator()
        {
            RuleFor(x => x.userId).NotEmpty().WithMessage("không được để trống!");
            RuleFor(x => x.code).NotEmpty().WithMessage("Không được để trống!")
                                .MaximumLength(50).WithMessage("Tối đa 50");
        }
    }
}
