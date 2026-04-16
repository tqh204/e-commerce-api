using Domain.Entities;
using Application.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Repositories
{
    public class CartRepository : ICartRepository
    {
        private readonly AppDBContext _context;
        
        public CartRepository(AppDBContext context) => _context = context;
        
        public async Task<Cart?> GetByCartIdAsync(Guid cartId) => await _context.Carts.Include(u => u.items).ThenInclude(u => u.product).FirstOrDefaultAsync(u => u.cartId == cartId); //Find a cart by cartId

        public async Task<Cart?> GetByUserIdAsync(Guid userId) => await _context.Carts.Include(u => u.items).ThenInclude(u => u.product).FirstOrDefaultAsync(u => u.userId == userId);  //Find a cart from a user through userId

        public async Task<CartItem?> GetCartItemByCartAndProductAsync(Guid cartId, Guid productId) => await _context.CartItems.FirstOrDefaultAsync(u => u.cartId == cartId && u.productId == productId); //Check if this product is already listed in a specific cart or can understand in another way like avoid dupicating

        public async Task<CartItem?> GetCartItemByIdAsync(Guid cartItemId) => await _context.CartItems.Include(u => u.cart).Include(u => u.product).FirstOrDefaultAsync(u => u.cartItemId == cartItemId);//Find a specific item in the cart by cartItemId

        public async Task AddCartAsync(Cart cart) => await _context.Carts.AddAsync(cart);

        public async Task AddCartItemAsync(CartItem cartItem) => await _context.CartItems.AddAsync(cartItem);

        public void RemoveCartItem(CartItem cartItem)
        {
            _context.CartItems.Remove(cartItem);    
        }

        public void UpdateCart(Cart cart)
        {
            _context.Carts.Update(cart);
        }

        public void UpdateCartItem(CartItem cartItem)
        {
            _context.CartItems.Update(cartItem);
        }

        public void RemoveCart(Cart cart)
        {
            _context.Carts.Remove(cart);
        }
    }
}
