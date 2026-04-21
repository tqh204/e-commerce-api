using Application.Features.Product.Queries.GetProductQuery;
using Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
namespace Application.Features.Variant.Queries
{
    public record GetProductVariantsQuery(
        Guid productId,
        int page = 1,
        int size = 10
    ) : IRequest<PagedResult<VariantDTO>>;
}
