using Domain.Entities;
using Application.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Features.Cart.Commands
{
    public class AddToCartCommandHandler : IRequestHandler<AddToCartCommand, Result<bool>>
    {
        private readonly ICartRepository _cartRepository;
        private readonly IProductRepository _productRepository;
        private readonly IUnitOfWork _unitOfWork;
        public AddToCartCommandHandler(ICartRepository cartRepository, IProductRepository productRepository, IUnitOfWork unitOfWork)
        {
            _cartRepository = cartRepository;
            _productRepository = productRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<bool>> Handle(AddToCartCommand request, CancellationToken cancellationToken)
        {
            var product = await _productRepository.GetProductByIdAsync(request.productId);

            if(product == null)
            {
                return Result<bool>.Failure("Không tìm thấy sản phẩm");
            }

            if(product.stockQuantity < request.quantity)
            {
                return Result<bool>.Failure("Số lượng trong kho không đủ");
            }

            var cart = await _cartRepository.GetByUserIdAsync(request.userId);

            var isNewCart = false;

            if(cart == null)
            {
                 cart = new Domain.Entities.Cart
                {
                    cartId = Guid.NewGuid(),
                    userId = request.userId,
                    createdAt = DateTime.UtcNow,
                    updatedAt = DateTime.UtcNow
                 };
                isNewCart = true;
                await _cartRepository.AddCartAsync(cart);
            }

            

            var existingItem = await _cartRepository.GetCartItemByCartAndProductAsync(cart.cartId, request.productId);
            if(existingItem != null)
            {
                var newQuantity = existingItem.quantity + request.quantity;
                if(product.stockQuantity < newQuantity)
                {
                    return Result<bool>.Failure("SỐ lượng trong kho không đủ");
                }
                existingItem.quantity = newQuantity;
                existingItem.updatedAt = DateTime.UtcNow;
                _cartRepository.UpdateCartItem(existingItem);
            }
            else
            {
                var carItem = new Domain.Entities.CartItem
                {
                    cartItemId = Guid.NewGuid(),
                    cartId = cart.cartId,
                    productId = product.productId,
                    quantity = request.quantity,
                    unitPrice = product.price,
                    createdAt = DateTime.UtcNow,
                    updatedAt = null
                };
                await _cartRepository.AddCartItemAsync(carItem);
            }
            cart.updatedAt = DateTime.UtcNow;
            if (!isNewCart)
            {
                _cartRepository.UpdateCart(cart);
            }
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result<bool>.Success(true);
        }
    }
}
