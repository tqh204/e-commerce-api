using Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Features.Cart.Commands
{
    public record UpdateCartItemCommand(Guid userId, Guid cartItemId, int quantity) : IRequest<Result<bool>>;

}
