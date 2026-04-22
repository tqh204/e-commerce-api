using System;
using System.Collections.Generic;
using System.Text;
using MediatR;
using Domain.Entities;
using Application.Common.Results;
namespace Application.Features.Product.Queries.GetProductQuery
{
    public record GetProductQuery(
        int page = 1,
        int size = 10,
        int? categoryId = null,
        decimal? minPrice = null,
        decimal? maxPrice = null) : IRequest<PagedResult<ProductDTO>>;
    
    
}

