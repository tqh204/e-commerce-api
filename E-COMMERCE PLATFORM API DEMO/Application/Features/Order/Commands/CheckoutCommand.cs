using Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using Application.Common.Results;

namespace Application.Features.Order.Commands
{
    public record CheckoutCommand(Guid userId) : IRequest<Result<Guid>>;//Thus type Guid here will return orderId
}

