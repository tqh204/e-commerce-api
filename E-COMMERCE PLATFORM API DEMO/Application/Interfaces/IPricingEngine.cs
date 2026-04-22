using Application.Common.Pricing;
using Domain.Entities;

namespace Application.Interfaces
{
    public interface IPricingEngine
    {
        Task<PricingResult> CalculatePromotionAsync(
            IReadOnlyList<CartItem> items,
            DateTime now,
            CancellationToken cancellationToken = default);
    }
}
