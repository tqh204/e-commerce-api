using Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Features.Cart.Queries
{
    public record GetCartQuery(Guid userId) : IRequest<Result<CartDTO>>;

}
