using Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using Application.Common.Results;

namespace Application.Features.Review.Commands
{
    public record CreateReviewCommand(Guid userId, Guid productId, int rating, string? comment) : IRequest<Result<Guid>>;
}

