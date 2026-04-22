using Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using Application.Common.Results;

namespace Application.Features.Cart.Queries
{
    public record GetCartQuery(Guid userId) : IRequest<Result<CartDTO>>;

}

