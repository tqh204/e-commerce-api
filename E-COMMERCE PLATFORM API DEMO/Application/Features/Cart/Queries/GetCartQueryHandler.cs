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

        public GetCartQueryHandler(ICartRepository cartRepository) => _cartRepository = cartRepository;


        public async Task<Result<CartDTO>> Handle(GetCartQuery request, CancellationToken cancellationToken)
        {
            var cart = await _cartRepository.GetByUserIdAsync(request.userId);
            if(cart == null)
            {
                var emptyCart = new CartDTO(
                    Guid.Empty,
                    request.userId,
                    new List<CartItemDTO>(),
                    0
                    );
                return Result<CartDTO>.Success(emptyCart);
            }
            var items = cart.items.Select(item => new CartItemDTO(
                item.cartItemId,
                item.productId,
                item.product?.productName ?? "",
                item.unitPrice,
                item.quantity,
                item.unitPrice * item.quantity)).ToList();

            var totalAmount = items.Sum(item => item.lineTotal);

            var cartDTO = new CartDTO(
                cart.cartId,
                cart.userId,
                items,
                totalAmount);
            return Result<CartDTO>.Success(cartDTO);

        }
    }
}
