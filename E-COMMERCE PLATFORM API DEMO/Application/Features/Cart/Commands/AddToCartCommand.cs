using Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using Application.Common.Results;

namespace Application.Features.Cart.Commands
{
    public record AddToCartCommand(Guid userId, Guid productId, int quantity) : IRequest<Result<bool>>;

}

