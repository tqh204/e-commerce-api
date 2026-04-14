using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Features.Product.Commands
{
    public class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
    {
        public CreateProductCommandValidator()
        {
            RuleFor(x => x.productName).NotEmpty().WithMessage("Không để trống nhé!")
                .MaximumLength(100).WithMessage("Không quá 100 ký tự!");
            RuleFor(x => x.price).GreaterThan(0).WithMessage("Không được âm và > 0");
            RuleFor(x => x.productDescription).MaximumLength(500).WithMessage("Không quá 500 ký tự").When(x => !string.IsNullOrWhiteSpace(x.productDescription));
            RuleFor(x => x.stockQuantity).GreaterThanOrEqualTo(0).WithMessage("Không âm nhé!");
            RuleFor(x => x.categoryId).GreaterThan(0).WithMessage("Điền hợp lệ");
        }
    }
}
