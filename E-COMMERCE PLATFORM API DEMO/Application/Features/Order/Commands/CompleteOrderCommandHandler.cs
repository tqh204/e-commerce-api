using Application.Interfaces;
using Domain.Entities;
using Domain.Enums;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using Application.Common.Results;

namespace Application.Features.Order.Commands
{
    public class CompleteOrderCommandHandler : IRequestHandler<CompleteOrderCommand, Result<bool>>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILoyaltyClient _loyaltyClient;

        public CompleteOrderCommandHandler(
            IOrderRepository orderRepository,
            IUserRepository userRepository,
            IUnitOfWork unitOfWork,
            ILoyaltyClient loyaltyClient)
        {
            _orderRepository = orderRepository;
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
            _loyaltyClient = loyaltyClient;
        }
        public async Task<Result<bool>> Handle(CompleteOrderCommand request, CancellationToken cancellationToken)
        {
            var order = await _orderRepository.GetOrderIdAsync(request.orderId);
            if (order == null)
            {
                return Result<bool>.Failure("Không tìm thấy order");
            }

            if (order.status == OrderStatus.Completed)
            {
                return Result<bool>.Failure("Order đã hoàn tất");
            }

            var user = order.user ?? await _userRepository.GetUserByIdAsync(order.userId);
            if (user == null)
            {
                return Result<bool>.Failure("Không tìm thấy user");
            }
            //Call the gRPC service to calculating loyalty points and update user ranks
            var loyaltyResult = await _loyaltyClient.CompleteOrderAsync(
                user.Id,
                user.loyaltyPoint,
                order.totalAmount,
                cancellationToken);

            order.status = OrderStatus.Completed;
            order.updatedAt = DateTime.UtcNow;

            user.loyaltyPoint = loyaltyResult.TotalPoints;
            user.rank = loyaltyResult.NewRank;

            _orderRepository.Update(order);
            _userRepository.Update(user);

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result<bool>.Success(true);
        }
    }
}
