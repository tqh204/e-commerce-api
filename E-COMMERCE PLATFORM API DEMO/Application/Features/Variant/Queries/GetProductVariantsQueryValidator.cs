using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Features.Variant.Queries
{
    public class GetProductVariantsQueryValidator : AbstractValidator<GetProductVariantsQuery>
    {
        public GetProductVariantsQueryValidator()
        {
            RuleFor(x => x.productId).NotEmpty();
            RuleFor(x => x.page).GreaterThan(0);
            RuleFor(x => x.size).InclusiveBetween(1, 100);
        }
    }
}
