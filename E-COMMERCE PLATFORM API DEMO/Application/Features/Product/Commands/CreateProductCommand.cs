using Application.Features.Product.Queries.GetProductQuery;
using Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using Application.Common.Results;

namespace Application.Features.Product.Commands
{
    public record CreateProductCommand(string productName, string? productDescription, decimal price, int stockQuantity, int categoryId) : IRequest<Result<Guid>>;
    
}

