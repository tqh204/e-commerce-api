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
            var cart = await _cartRepository.GetByUserIdAsync(request.userId);//Take cart of user by userId
            if(cart == null)//if user has not had cart yet, create a new one
            {
                var emptyCart = new CartDTO(//As variable name, it will create a empty cart for user to contain item in here
                    Guid.Empty,
                    request.userId,
                    new List<CartItemDTO>(),
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

            var cartDTO = new CartDTO(//create new cartDTO to update data to return to user
                cart.cartId,
                cart.userId,
                items,
                totalAmount);
            return Result<CartDTO>.Success(cartDTO);

        }
    }
}
