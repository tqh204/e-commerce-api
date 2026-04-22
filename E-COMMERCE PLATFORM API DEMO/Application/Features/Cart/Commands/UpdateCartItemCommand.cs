using Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using Application.Common.Results;

namespace Application.Features.Cart.Commands
{
    public record UpdateCartItemCommand(Guid userId, Guid cartItemId, int quantity) : IRequest<Result<bool>>;

}

