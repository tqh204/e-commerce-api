using Application.Interfaces;
using Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

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
            var cartItem = await _cartRepository.GetCartItemByIdAsync(request.cartItemId);
            if(cartItem == null)
            {
                return Result<bool>.Failure("Không tìm thấy sản phẩm trong dỏ hàng nho~~");
            }

            if(cartItem == null || cartItem.cart.userId != request.userId)
            {
                return Result<bool>.Failure("Bạn không có quyền cập nhật sản phẩm nì nho~~");
            }

            if(request.quantity == 0)
            {
                cartItem.cart.updatedAt = DateTime.UtcNow;
                _cartRepository.RemoveCartItem(cartItem);
                _cartRepository.UpdateCart(cartItem.cart);

                await _unitOfWork.SaveChangesAsync(cancellationToken);
                return Result<bool>.Success(true);
            }

            if(cartItem.product == null)
            {
                return Result<bool>.Failure("Không tìm thấy sản phẩm");
            }

            if(cartItem.product.stockQuantity < request.quantity)
            {
                return Result<bool>.Failure("Số lượng trong kho không đủ");
            }

            cartItem.quantity = request.quantity;
            cartItem.updatedAt = DateTime.UtcNow;
            cartItem.cart.updatedAt = DateTime.UtcNow;

            _cartRepository.UpdateCartItem(cartItem);
            _cartRepository.UpdateCart(cartItem.cart);

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result<bool>.Success(true);
        }
    }
}
