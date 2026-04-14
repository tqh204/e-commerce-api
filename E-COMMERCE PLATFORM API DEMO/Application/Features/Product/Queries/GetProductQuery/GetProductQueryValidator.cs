using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Features.Product.Queries.GetProductQuery
{
    public class GetProductQueryValidator : AbstractValidator<GetProductQuery>
    {
        public GetProductQueryValidator() {
            RuleFor(x => x.page).GreaterThanOrEqualTo(1).WithMessage("Page phải lớn hơn hoặc bằng 1");
            RuleFor(x => x.size).InclusiveBetween(1, 100).WithMessage("Size phải nằm khoảng từ 1 - 100");
            RuleFor(x => x.minPrice).GreaterThanOrEqualTo(0).When(x => x.minPrice.HasValue).WithMessage("minPrice không âm");
            RuleFor(x => x.maxPrice).GreaterThanOrEqualTo(0).When(x => x.maxPrice.HasValue).WithMessage("maxPrice không âm");
            RuleFor(x => x).Must(x => !x.minPrice.HasValue || !x.maxPrice.HasValue || x.minPrice <= x.maxPrice).WithMessage("minPrice phải nhỏ hơn hoặc bằng maxPrice");
        }
    }
}
