using Domain.Entities;
using Application.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Features.Cart.Commands
{
    public class AddToCartCommandHandler : IRequestHandler<AddToCartCommand, Result<bool>>//Resolve on the left, and return the result on the right
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
            var product = await _productRepository.GetProductByIdAsync(request.productId); //Find the product if it has already in DB

            if(product == null)//Return null if product not found
            {
                return Result<bool>.Failure("Không tìm thấy sản phẩm");
            }

            if(product.stockQuantity < request.quantity)//return failif request > stock
            {
                return Result<bool>.Failure("Số lượng trong kho không đủ");
            }

            var cart = await _cartRepository.GetByUserIdAsync(request.userId);//Finding the cart of user is active(login)

            //var isNewCart = false;//create this variable to know this user has already had a cart or not yet?
            var now = DateTime.UtcNow;
            if(cart == null)//If could not find cart of user is active, create new cart for user
            {
                 cart = new Domain.Entities.Cart
                {
                    cartId = Guid.NewGuid(),
                    userId = request.userId,
                    createdAt = now,
                    updatedAt = now
                 };
                //isNewCart = true;//So if the cart of user is new => Add
                await _cartRepository.AddCartAsync(cart);
            }
            else
            {
                cart.updatedAt = now;
                //_cartRepository.UpdateCart(cart);
            }

            

            var existingItem = await _cartRepository.GetCartItemByCartAndProductAsync(cart.cartId, request.productId);//Checking in this cart(cartId) has had this product(productId)?
            if(existingItem != null)//If the product has already in this cart(cartId) => create a new variable to contain the present quantity(existingItem.quantity) + request from user(request.quantity)
            {
                var newQuantity = existingItem.quantity + request.quantity;
                if(product.stockQuantity < newQuantity)//So add is not just an add function, its also a update or u could get it as "add more"
                {
                    return Result<bool>.Failure("SỐ lượng trong kho không đủ");//And when u add more but the stock could have not enough quantity(stock), it will be fail
                }
                existingItem.quantity = newQuantity;//if it got enough stock(quantity) => the present quantity(existing.quantity) = new quantity(newQuantity)
                existingItem.updatedAt = DateTime.UtcNow;
                _cartRepository.UpdateCartItem(existingItem);//using update here cuz u add more mean u are using update not add method anymore.
            }
            else
            {
                var carItem = new Domain.Entities.CartItem//And if the product has not been in this cart? => create cartItem for this product in this cart
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
            //cart.updatedAt = DateTime.UtcNow;//Old cart will be updated in this row, but new cart no need to updated cuz its updated at the same time created
            //if (!isNewCart)
            //{
            //    _cartRepository.UpdateCart(cart);
            //}
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result<bool>.Success(true);
        }
    }
}
