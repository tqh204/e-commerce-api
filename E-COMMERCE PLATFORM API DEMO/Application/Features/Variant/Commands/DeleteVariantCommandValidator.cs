using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Features.Variant.Commands
{
    public class DeleteVariantCommandValidator : AbstractValidator<DeleteVariantCommand>
    {
        public DeleteVariantCommandValidator()
        {
            RuleFor(x => x.variantId).NotEmpty();
        }
    }
}
