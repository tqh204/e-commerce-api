using Application.Common.Loyalty;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces
{
    public interface ILoyaltyClient
    {
        Task<LoyaltyPreviewResult> PreviewBenefitsAsync(
            Guid userId,
            int currentPoints,  
            decimal subtotalAfterCoupon,
            CancellationToken cancellationToken = default);

        Task<LoyaltyCompleteResult> CompleteOrderAsync(
            Guid userId,
            int currentPoints,
            decimal completedOrderAmount,
            CancellationToken cancellationToken = default);
    }
}
