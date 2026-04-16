using System;
using System.Collections.Generic;
using System.Text;
using MediatR;
namespace Application.Features.Order.Queries
{
    public record GetOrderQuery(Guid userId) : IRequest<IReadOnlyList<OrderDTO>>;

}
