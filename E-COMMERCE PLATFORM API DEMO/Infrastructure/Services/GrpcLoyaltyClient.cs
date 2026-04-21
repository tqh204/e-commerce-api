using Application.Common.Loyalty;
using Application.Interfaces;
using Domain.Enums;
using Loyalty.Grpc;

namespace Infrastructure.Services
{
    public class GrpcLoyaltyClient : ILoyaltyClient
    {
        private readonly LoyaltyService.LoyaltyServiceClient _client;

        public GrpcLoyaltyClient(LoyaltyService.LoyaltyServiceClient client) => _client = client;

        public async Task<LoyaltyPreviewResult> PreviewBenefitsAsync(Guid userId, int currentPoints, decimal subtotalAfterCoupon, CancellationToken cancellationToken = default)
        {
            var response = await _client.PreviewBenefitsAsync(new PreviewBenefitsRequest //Package and send the request to the gRPC service
            {
                UserId = userId.ToString(),//proto doesn't support Guid, so we convert it to string
                CurrentPoints = currentPoints,
                SubtotalAfterCoupon = (double)subtotalAfterCoupon//proto doesn't support decimal, so convert it to double
            }, cancellationToken: cancellationToken);

            return new LoyaltyPreviewResult(//Convert response from proto to our application model
                MapRank(response.Rank),//LoyaltyRankProto -> loyaltyRank
                (decimal)response.RankDiscountPercent,//double -> decimal
                (decimal)response.RankDiscountAmount
            );
        }

        public async Task<LoyaltyCompleteResult> CompleteOrderAsync(Guid userId, int currentPoints, decimal completedOrderAmount, CancellationToken cancellationToken = default)
        {
            var response = await _client.CompleteOrderAsync(new CompleteOrderRequest
            {
                UserId = userId.ToString(),
                CurrentPoints = currentPoints,
                CompletedOrderAmount = (double)completedOrderAmount
            }, cancellationToken: cancellationToken);

            return new LoyaltyCompleteResult(
                response.EarnedPoints,
                response.TotalPoints,
                MapRank(response.NewRank),
                (decimal)response.NextRankDiscountPercent
            );
        }

        private static LoyaltyRank MapRank(LoyaltyRankProto rank) => rank switch
        {
            LoyaltyRankProto.Silver => LoyaltyRank.Silver,
            LoyaltyRankProto.Gold => LoyaltyRank.Gold,
            LoyaltyRankProto.Diamond => LoyaltyRank.Diamond,
            _ => LoyaltyRank.Bronze
        };
    }
}
