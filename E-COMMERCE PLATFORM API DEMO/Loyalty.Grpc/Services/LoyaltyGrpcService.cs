using Grpc.Core;

namespace Loyalty.Grpc.Services
{
    public class LoyaltyGrpcService : LoyaltyService.LoyaltyServiceBase
    {
        public override Task<PreviewBenefitsResponse> PreviewBenefits(
            PreviewBenefitsRequest request,
            ServerCallContext context)
        {
            var rank = CalculateRank(request.CurrentPoints);
            var percent = GetRankDiscountPercent(rank);
            var amount = request.SubtotalAfterCoupon * percent / 100.0;

            return Task.FromResult(new PreviewBenefitsResponse
            {
                Rank = rank,
                RankDiscountPercent = percent,
                RankDiscountAmount = amount
            });
        }

        public override Task<CompleteOrderResponse> CompleteOrder(
            CompleteOrderRequest request,
            ServerCallContext context)
        {
            var earnedPoints = (int)(request.CompletedOrderAmount / 100000);
            var totalPoints = request.CurrentPoints + earnedPoints;
            var newRank = CalculateRank(totalPoints);
            var nextPercent = GetRankDiscountPercent(newRank);

            return Task.FromResult(new CompleteOrderResponse
            {
                EarnedPoints = earnedPoints,
                TotalPoints = totalPoints,
                NewRank = newRank,
                NextRankDiscountPercent = nextPercent
            });
        }

        private static LoyaltyRankProto CalculateRank(int points)
        {
            if (points >= 5000) return LoyaltyRankProto.Diamond;
            if (points >= 1000) return LoyaltyRankProto.Gold;
            if (points >= 500) return LoyaltyRankProto.Silver;
            return LoyaltyRankProto.Bronze;
        }

        private static double GetRankDiscountPercent(LoyaltyRankProto rank)
        {
            return rank switch
            {
                LoyaltyRankProto.Silver  => 2,
                LoyaltyRankProto.Gold => 5,
                LoyaltyRankProto.Diamond => 10,
                _ => 0
            };
        }
    }
}
