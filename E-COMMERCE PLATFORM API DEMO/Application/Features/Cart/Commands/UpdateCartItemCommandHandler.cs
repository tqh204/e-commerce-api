using Application.Interfaces;
using Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using Application.Common.Results;

namespace Application.Features.Cart.Commands
{
    public class UpdateCartItemCommandHandler : IRequestHandler<UpdateCartItemCommand, Result<bool>>
    {
        private readonly ICartRepository _cartRepository;
        private readonly IUnitOfWork _unitOfWork;
        public UpdateCartItemCommandHandler(ICartRepository cartRepository, IUnitOfWork unitOfWork)
        {
            _cartRepository = cartRepository;
            _unitOfWork = unitOfWork;
        }
        public async Task<Result<bool>> Handle(UpdateCartItemCommand request, CancellationToken cancellationToken)
        {
            var cartItem = await _cartRepository.GetCartItemByIdAsync(request.cartItemId);//Searching cartItem by cartItemId
            if(cartItem == null)//Not found
            {
                return Result<bool>.Failure("Không tìm th?y s?n ph?m trong d? hàng nho~~");
            }

            if(cartItem == null || cartItem?.cart?.userId != request.userId)//notfound or dont have permission to get into another cart's user
            {
                return Result<bool>.Failure("B?n không có quy?n c?p nh?t s?n ph?m nì nho~~");
            }

            if(request.quantity == 0)//Setting if in cart, itemCart quantity = 0 => remove it out of cart
            {
                cartItem.cart.updatedAt = DateTime.UtcNow;//update time
                cartItem.cart.couponId = null;
                cartItem.cart.couponCode = null;
                cartItem.cart.discountAmount = 0;
                _cartRepository.RemoveCartItem(cartItem);//remove cartItem out of cart
                _cartRepository.UpdateCart(cartItem.cart);//Throw it into AppDBContext to prepare command update to save in DB

                await _unitOfWork.SaveChangesAsync(cancellationToken);//Save into DB
                return Result<bool>.Success(true);
            }

            if(cartItem.product == null)//Make sure the product is still existing in cart in case quantity > 0
            {
                return Result<bool>.Failure("Không tìm th?y s?n ph?m");
            }

            if(cartItem.product.stockQuantity < request.quantity)//Compared between quantity in DB and newQuantity from user
            {
                return Result<bool>.Failure("S? lu?ng trong kho không d?");
            }

            cartItem.quantity = request.quantity;
            cartItem.updatedAt = DateTime.UtcNow;
            cartItem.cart.updatedAt = DateTime.UtcNow;
            cartItem.cart.couponId = null;
            cartItem.cart.couponCode = null;
            cartItem.cart.discountAmount = 0;

            _cartRepository.UpdateCartItem(cartItem);
            _cartRepository.UpdateCart(cartItem.cart);

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result<bool>.Success(true);
        }
    }
}

