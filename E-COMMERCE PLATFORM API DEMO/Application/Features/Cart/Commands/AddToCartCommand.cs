using Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Features.Cart.Commands
{
    public record AddToCartCommand(Guid userId, Guid productId, int quantity) : IRequest<Result<bool>>;

}
