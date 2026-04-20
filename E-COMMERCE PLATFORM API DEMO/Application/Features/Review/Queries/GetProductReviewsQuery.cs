using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Features.Review.Queries
{
    public record GetProductReviewsQuery(
        Guid ProductId,
        int Page = 1,
        int Size = 10
    ) : IRequest<ReviewPagedResult>;

}
