using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Features.Product.Commands
{
    public class DeleteProductCommandValidator : AbstractValidator<DeleteProductCommand>
    {
        public DeleteProductCommandValidator()
        {
            RuleFor(x => x.productId)
                .NotEmpty().WithMessage("Id sản phẩm khống đúng!");
        }
    }
}
