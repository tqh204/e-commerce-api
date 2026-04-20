using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Common.Loyalty
{
    public record LoyaltyCompleteResult(
        int EarnedPoints,
        int TotalPoints,
        LoyaltyRank NewRank,
        decimal NextRankDiscountPercent);
}
