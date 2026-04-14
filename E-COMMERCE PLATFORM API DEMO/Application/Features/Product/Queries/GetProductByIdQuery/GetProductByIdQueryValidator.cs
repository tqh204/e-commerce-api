using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Features.Product.Queries.GetProductByIdQuery
{
    public class GetProductByIdQueryValidator : AbstractValidator<GetProductByIdQuery>
    {
        public GetProductByIdQueryValidator()
        {
            RuleFor(x => x.productId).NotEmpty().WithMessage("Id không được để trống nhe~~");
        }
    }
}
