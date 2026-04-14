using Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Features.Product.Commands
{
    public record UpdateProductCommand(Guid productId, string productName, string? productDescription, decimal price, int stockQuantity, int categoryId) : IRequest<Result<bool>>;
   
    
}
