using Application.Interfaces;
using Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Features.Order.Commands
{
    public class CheckoutCommandHandler : IRequestHandler<CheckoutCommand, Result<Guid>>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly ICartRepository _cartRepository;
        private readonly IUnitOfWork _unitOfWork;
        public CheckoutCommandHandler(IOrderRepository orderRepository, ICartRepository cartRepository, IUnitOfWork unitOfWork)
        {
            _orderRepository = orderRepository;
            _cartRepository = cartRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<Guid>> Handle(CheckoutCommand request, CancellationToken cancellationToken)
        {
            var cart = await _cartRepository.GetByUserIdAsync(request.userId);//Taking cart's user 
            if(cart == null || !cart.items.Any())
            {
                return Result<Guid>.Failure("Không tìm thấy cart hoặc không có item trong cart");
            }
            await _unitOfWork.BeginTransactionAsync(cancellationToken);//Open transaction
            try
            {
                foreach(var item in cart.items)//Starting a loop to check whether any item in cart rn?
                {
                    if(item.product == null)//Checking whether item in cart? if not => rollback
                    {
                        await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                        return Result<Guid>.Failure("Không tìm thấy sản phẩm trong giỏ hàng");
                    }

                    if(item.product.stockQuantity < item.quantity)//Checking if product stockquantity < item quantity => rollback
                    {
                        await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                        return Result<Guid>.Failure($"Sản phẩm {item.product.productName} không đủ hàng");
                    }
                }

                var order = new Domain.Entities.Order//If pass, create a new order
                {
                    orderId = Guid.NewGuid(), //Create an id for order by Guid
                    userId = request.userId,//take the userId from request to make sure that the system is creating an order for the right user
                    totalAmount = cart.items.Sum(x => x.unitPrice * x.quantity),//take the price of item * quantity of item 
                    status = "PENDING",
                    createdAt = DateTime.UtcNow,
                    updatedAt = null
                };

                foreach(var item in cart.items)//start a loop to check each item in cart item
                {
                    item.product!.stockQuantity -= item.quantity;//Caculating the rest stockQuantity of product
                    item.product.updatedAt = DateTime.UtcNow;

                    var orderItem = new OrderItem//create new item of order to prepare to save in DB
                    {
                        orderItemId = Guid.NewGuid(),
                        orderId = order.orderId,
                        productId = item.productId,
                        productName = item.product.productName,
                        quantity = item.quantity,
                        unitPrice = item.unitPrice,
                        lineTotal = item.unitPrice * item.quantity,
                    };
                    order.items.Add(orderItem);//add items into Order
                }
                await _orderRepository.AddAsync(order);
                _cartRepository.RemoveCart(cart);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                await _unitOfWork.CommitTransactionAsync(cancellationToken);
                return Result<Guid>.Success(order.orderId);
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                throw;
            }

        }
    }
}
