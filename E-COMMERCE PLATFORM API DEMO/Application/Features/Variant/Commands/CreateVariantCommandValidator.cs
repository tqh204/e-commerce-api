using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Features.Variant.Commands
{
    public class CreateVariantCommandValidator : AbstractValidator<CreateVariantCommand>
    {
        public CreateVariantCommandValidator()
        {
            RuleFor(x => x.productId).NotEmpty();
            RuleFor(x => x.sku).NotEmpty().MaximumLength(100);
            RuleFor(x => x.price).GreaterThan(0);
            RuleFor(x => x.inventory).GreaterThanOrEqualTo(0);
            RuleFor(x => x)
                .Must(x => !string.IsNullOrWhiteSpace(x.size) ||
                           !string.IsNullOrWhiteSpace(x.color) ||
                           !string.IsNullOrWhiteSpace(x.material))
                .WithMessage("Phải có ít nhất một thuộc tính biến thể: size/color/material.");
        }
    }
}
