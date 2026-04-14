using Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Features.Product.Commands
{
    public record DeleteProductCommand(Guid productId) : IRequest<Result<bool>>;
    
    
}
