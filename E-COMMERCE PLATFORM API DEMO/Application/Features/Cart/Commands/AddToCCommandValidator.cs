using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Features.Cart.Commands
{
    public class AddToCommandValidator : AbstractValidator<AddToCartCommand>
    {
        public AddToCommandValidator()
        {
            RuleFor(x => x.userId).NotEmpty().WithMessage("UserId không hợp lệ");
            RuleFor(x => x.productId).NotEmpty().WithMessage("ProductId không hợp lệ");
            RuleFor(x => x.quantity).GreaterThan(0).WithMessage("Số lượng phải lớn hơn 0");
        }
    }
}
