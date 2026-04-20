using FluentValidation;
using FluentValidation.Validators;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Features.Coupon.Commands
{
    public class CreateCouponCommandValidator : AbstractValidator<CreateCouponCommand>
    {
        public CreateCouponCommandValidator()
        {
            RuleFor(x => x.code)
                .NotEmpty().WithMessage("Code không được để trống")
                .MaximumLength(50).WithMessage("Code tối đa 50 ký tự");

            RuleFor(x => x.discountType)
                .IsInEnum()
                .WithMessage("DiscountType không hợp lệ");


            RuleFor(x => x.value)
                .GreaterThan(0).WithMessage("Value phải lớn hơn 0");

            RuleFor(x => x.minOrderValue)
                .GreaterThanOrEqualTo(0).WithMessage("MinOrderValue không được âm");

            RuleFor(x => x.usageLimit)
                .GreaterThan(0).WithMessage("UsageLimit phải lớn hơn 0");

            RuleFor(x => x.endDate)
                .GreaterThan(x => x.startDate)
                .WithMessage("EndDate phải lớn hơn StartDate");
        }
    }
}
