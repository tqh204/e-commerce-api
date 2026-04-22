using Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using Application.Common.Results;

namespace Application.Features.Order.Commands
{
    public record CompleteOrderCommand(Guid userId, Guid orderId) : IRequest<Result<bool>>;
}

