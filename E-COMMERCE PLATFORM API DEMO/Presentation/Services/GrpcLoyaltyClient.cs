using Application.Common.Loyalty;
using Application.Interfaces;
using Domain.Enums;
using Loyalty.Grpc;

namespace Presentation.Services
{
    public class GrpcLoyaltyClient : ILoyaltyClient
    {
        private readonly LoyaltyService.LoyaltyServiceClient _client;

        public GrpcLoyaltyClient(LoyaltyService.LoyaltyServiceClient client) => _client = client;

        public async Task<LoyaltyPreviewResult> PreviewBenefitsAsync(Guid userId, int currentPoints, decimal subtotalAfterCoupon, CancellationToken cancellationToken = default)
        {
            var response = await _client.PreviewBenefitsAsync(new PreviewBenefitsRequest
            {
                UserId = userId.ToString(),
                CurrentPoints = currentPoints,
                SubtotalAfterCoupon = (double)subtotalAfterCoupon
            }, cancellationToken: cancellationToken);

            return new LoyaltyPreviewResult(
                MapRank(response.Rank),
                (decimal)response.RankDiscountPercent,
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
