using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Features.Cart.Commands
{
    public class UpdateCartItemCommandValidator : AbstractValidator<UpdateCartItemCommand>
    {
        public UpdateCartItemCommandValidator()
        {
            RuleFor(x => x.userId)
              .NotEmpty()
              .WithMessage("UserId không hợp lệ");

            RuleFor(x => x.cartItemId)
                .NotEmpty()
                .WithMessage("CartItemId không hợp lệ");

            RuleFor(x => x.quantity)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Số lượng phải lớn hơn hoặc bằng 0");
        }
    }
}
