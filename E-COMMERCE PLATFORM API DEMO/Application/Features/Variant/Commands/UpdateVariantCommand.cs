using Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Features.Variant.Commands
{
    public record UpdateVariantCommand(
       Guid variantId,
       string sku,
       string? size,
       string? color,
       string? material,
       decimal price,
       int inventory
   ) : IRequest<Result<bool>>;
}
