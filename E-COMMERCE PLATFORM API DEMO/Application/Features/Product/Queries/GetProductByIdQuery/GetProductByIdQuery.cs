using Application.Common.Results;
    using Application.Features.Product.Queries.GetProductQuery;
using Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Features.Product.Queries.GetProductByIdQuery
{
    public record GetProductByIdQuery(Guid productId) : IRequest<Result<ProductDTO>>;
    
    
}

