using Application.Interfaces;
using Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Features.Cart.Queries
{
    public class GetCartQueryHandler : IRequestHandler<GetCartQuery, Result<CartDTO>>
    {
        private readonly ICartRepository _cartRepository;
        private readonly IPricingEngine _pricingEngine;

        public GetCartQueryHandler(ICartRepository cartRepository, IPricingEngine pricingEngine)
        {
            _cartRepository = cartRepository;
            _pricingEngine = pricingEngine;
        }


        public async Task<Result<CartDTO>> Handle(GetCartQuery request, CancellationToken cancellationToken)
        {
            var cart = await _cartRepository.GetByUserIdAsync(request.userId);//Take cart of user by userId
            if(cart == null)//if user has not had cart yet, create a new one
            {
                var emptyCart = new CartDTO(//As variable name, it will create a empty cart for user to contain item in here
                    Guid.Empty,
                    request.userId,
                    new List<CartItemDTO>(),
                    0,
                    0,
                    null,
                    0,
                    0,
                    0
                    );
                return Result<CartDTO>.Success(emptyCart);
            }
            var items = cart.items.Select(item => new CartItemDTO(//and if user has already had a cart, convert cartItem from entity to cartItem from cartItemDTO. Why do not use cartItem of entity straight?
                item.cartItemId,//Cuz it would custom a suitable data for user, avoid attributes no need to show.
                item.productId,
                item.product?.productName ?? "",
                item.unitPrice,
                item.quantity,
                item.unitPrice * item.quantity)).ToList();

            var totalAmount = items.Sum(item => item.lineTotal);//This is a sum amount of cart

            var promotionResult = await _pricingEngine.CalculatePromotionAsync(cart.items.ToList(), DateTime.UtcNow, cancellationToken);
            var promotionDiscountAmount = Math.Max(0, Math.Min(promotionResult.discountAmount, totalAmount));
            var afterPromotion = totalAmount - promotionDiscountAmount;

            var couponDiscountAmount = Math.Max(0, Math.Min(cart.discountAmount, afterPromotion));
            var discountAmount = couponDiscountAmount; // backward-compatible alias

            var finalAmount = afterPromotion - couponDiscountAmount;
            if(finalAmount < 0)
            {
                finalAmount = 0;
            }
            var cartDTO = new CartDTO(//create new cartDTO to update data to return to user
                cart.cartId,
                cart.userId,
                items,
                totalAmount,
                promotionDiscountAmount,
                cart.couponCode,
                couponDiscountAmount,
                discountAmount,
                finalAmount);
            return Result<CartDTO>.Success(cartDTO);

        }
    }
}
