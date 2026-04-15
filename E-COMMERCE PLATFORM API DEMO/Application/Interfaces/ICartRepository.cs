using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces
{
    public interface ICartRepository
    {
        Task<Cart?> GetByUserIdAsync(Guid userId);
        Task<Cart?> GetByCartIdAsync(Guid cartId);
        Task<CartItem?> GetCartItemByIdAsync(Guid cartItemId);
        Task<CartItem?> GetCartItemByCartAndProductAsync(Guid cartId, Guid productId);
        Task AddCartAsync(Cart cart);
        Task AddCartItemAsync(CartItem cartItem);

        void UpdateCart(Cart cart);
        void UpdateCartItem(CartItem cartItem);
        void RemoveCartItem(CartItem cartItem);
    }
}
