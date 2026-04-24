using Application.Common.Results;
using Application.Interfaces;
using Domain.Entities;
using Domain.Enums;

namespace Application.Common.PayOS.Service
{
    public class CheckoutSnapshotService : ICheckoutSnapshotService
    {
        private readonly ICartRepository _cartRepository;
        private readonly ICouponRepository _couponRepository;
        private readonly IPricingEngine _pricingEngine;
        private readonly ILoyaltyClient _loyaltyClient;
        private readonly IUserRepository _userRepository;

        public CheckoutSnapshotService(
            ICartRepository cartRepository,
            ICouponRepository couponRepository,
            IPricingEngine pricingEngine,
            ILoyaltyClient loyaltyClient,
            IUserRepository userRepository)
        {
            _cartRepository = cartRepository;
            _couponRepository = couponRepository;
            _pricingEngine = pricingEngine;
            _loyaltyClient = loyaltyClient;
            _userRepository = userRepository;
        }

        public async Task<Result<CheckoutSnapshot>> BuildAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            var cart = await _cartRepository.GetByUserIdAsync(userId);
            if (cart == null || !cart.items.Any())
            {
                return Result<CheckoutSnapshot>.Failure("Không tìm thấy cart hoặc không có item trong cart");
            }

            foreach (var item in cart.items)
            {
                if (item.product == null)
                {
                    return Result<CheckoutSnapshot>.Failure("Không tìm thấy sản phẩm trong giỏ hàng");
                }

                if (item.product.stockQuantity < item.quantity)
                {
                    return Result<CheckoutSnapshot>.Failure($"Sản phẩm {item.product.productName} không đủ hàng");
                }
            }

            Coupon? coupon = null;
            decimal couponDiscount = 0;

            var subTotal = cart.items.Sum(x => x.unitPrice * x.quantity);
            var promotionResult = await _pricingEngine.CalculatePromotionAsync(cart.items.ToList(), DateTime.UtcNow, cancellationToken);
            var promotionDiscount = promotionResult.discountAmount;
            var afterPromotion = Math.Max(0, subTotal - promotionDiscount);

            if (cart.couponId.HasValue)
            {
                coupon = await _couponRepository.GetCouponByIdAsync(cart.couponId.Value);
                if (coupon == null)
                {
                    return Result<CheckoutSnapshot>.Failure("Không tìm thấy code");
                }

                var now = DateTime.UtcNow;
                if (!coupon.isActive)
                {
                    return Result<CheckoutSnapshot>.Failure("Code chưa hoạt động");
                }

                if (coupon.startDate > now)
                {
                    return Result<CheckoutSnapshot>.Failure("Code hiện chưa khả dụng");
                }

                if (coupon.endDate < now)
                {
                    return Result<CheckoutSnapshot>.Failure("Code hiện không khả dụng");
                }

                if (coupon.usedCount >= coupon.usageLimit)
                {
                    return Result<CheckoutSnapshot>.Failure("Code đã hết lượt sử dụng");
                }

                if (afterPromotion < coupon.minOrderValue)
                {
                    return Result<CheckoutSnapshot>.Failure("Chưa đủ điều kiện để sử dụng");
                }

                couponDiscount = coupon.discountType switch
                {
                    DiscountType.Percentage => afterPromotion * (coupon.value / 100m),
                    DiscountType.FixedAmount => coupon.value,
                    _ => 0
                };
                couponDiscount = Math.Max(0, Math.Min(couponDiscount, afterPromotion));
            }

            var afterCoupon = Math.Max(0, afterPromotion - couponDiscount);
            var user = await _userRepository.GetUserByIdAsync(userId);
            if (user == null)
            {
                return Result<CheckoutSnapshot>.Failure("Không tìm thấy user");
            }

            var loyaltyPreview = await _loyaltyClient.PreviewBenefitsAsync(
                userId,
                user.loyaltyPoint,
                afterCoupon,
                cancellationToken);

            var rankDiscount = loyaltyPreview.RankDiscountAmount;
            var finalTotal = Math.Max(0, afterCoupon - rankDiscount);
            finalTotal = decimal.Round(finalTotal, 0, MidpointRounding.AwayFromZero);

            var snapshot = new CheckoutSnapshot(
                userId,
                coupon?.couponId,
                coupon?.code,
                subTotal,
                promotionDiscount,
                couponDiscount,
                rankDiscount,
                finalTotal,
                cart.items.Select(item => new CheckoutSnapshotItem(
                    item.productId,
                    item.product!.productName,
                    item.quantity,
                    item.unitPrice,
                    item.unitPrice * item.quantity)).ToList());

            return Result<CheckoutSnapshot>.Success(snapshot);
        }
    }
}
