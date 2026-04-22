using Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using Application.Common.Results;

namespace Application.Features.Product.Commands
{
    public record DeleteProductCommand(Guid productId) : IRequest<Result<bool>>;
    
    
}

