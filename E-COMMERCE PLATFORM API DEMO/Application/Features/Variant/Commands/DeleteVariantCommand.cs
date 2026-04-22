using Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using Application.Common.Results;

namespace Application.Features.Variant.Commands
{
    public record DeleteVariantCommand(Guid variantId) : IRequest<Result<bool>>;

}

