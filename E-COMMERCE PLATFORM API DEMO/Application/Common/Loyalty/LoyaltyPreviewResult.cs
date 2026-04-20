using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Common.Loyalty
{
    public record LoyaltyPreviewResult(
        LoyaltyRank Rank,
        decimal RankDiscountPercent,
        decimal RankDiscountAmount);
}
