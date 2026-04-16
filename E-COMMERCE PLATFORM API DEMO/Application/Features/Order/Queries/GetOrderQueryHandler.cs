using Application.Interfaces;
using Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Features.Order.Queries
{
    public class GetOrderQueryHandler : IRequestHandler<GetOrderQuery, IReadOnlyList<OrderDTO>>
    {
        private readonly IOrderRepository _orderRepository;
        public GetOrderQueryHandler(IOrderRepository orderRepository) => _orderRepository = orderRepository;
        public async Task<IReadOnlyList<OrderDTO>> Handle(GetOrderQuery request, CancellationToken cancellationToken)
        {
            var orders = await _orderRepository.GetOrderByUserId(request.userId);

            var result = orders.Select(order => new OrderDTO(
                order.orderId,
                order.totalAmount,
                order.status,
                order.createdAt,
                order.items.Select(item => new OrderItemDTO(
                    item.productId,
                    item.productName,
                    item.quantity,
                    item.unitPrice,
                    item.lineTotal)).ToList()
                    )).ToList();
            return result;
        }
    }
}
