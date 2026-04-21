using Grpc.Core;

namespace Loyalty.Grpc.Services
{
    public class LoyaltyGrpcService : LoyaltyService.LoyaltyServiceBase//This class implements the gRPC service defined in the .proto file
    {
        public override Task<PreviewBenefitsResponse> PreviewBenefits(//Just review the benifits for the user first before the user complete the order.
            PreviewBenefitsRequest request,
            ServerCallContext context)//Why when a method contains a request parameter, but also contains a context parameter, the method is still considered a gRPC service method? Because in gRPC, the service methods are defined to take both a request message and a ServerCallContext. The request message contains the data sent by the client, while the ServerCallContext provides information about the call, such as headers, cancellation tokens, and other metadata. This allows the service method to access both the client's data and the context of the call, enabling it to handle requests effectively.
        {
            var rank = CalculateRank(request.CurrentPoints);//caculating the currently rank by using the current points 
            var percent = GetRankDiscountPercent(rank);//getting the discount percentage based on the calculated rank(Rank currently of user)
            var amount = request.SubtotalAfterCoupon * percent / 100.0;//Caculating the discount amount based on the subtotal after coupon and the discount percentage

            return Task.FromResult(new PreviewBenefitsResponse
            {
                Rank = rank,
                RankDiscountPercent = percent,
                RankDiscountAmount = amount
            });
        }

        public override Task<CompleteOrderResponse> CompleteOrder(//Caculating for the user after they complete the order.
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

        private static LoyaltyRankProto CalculateRank(int points)//This method calculates the loyalty rank based on the total points
        {
            if (points >= 5000) return LoyaltyRankProto.Diamond;
            if (points >= 1000) return LoyaltyRankProto.Gold;
            if (points >= 500) return LoyaltyRankProto.Silver;
            return LoyaltyRankProto.Bronze;
        }

        private static double GetRankDiscountPercent(LoyaltyRankProto rank)//This method returns the discount percentage based on the loyalty rank
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
