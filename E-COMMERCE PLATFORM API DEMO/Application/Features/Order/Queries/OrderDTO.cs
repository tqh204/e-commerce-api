using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Features.Order.Queries
{
    public record OrderDTO(
        Guid orderId,
        decimal totalAmount,
        string status,
        DateTime createdAt,
        IReadOnlyList<OrderItemDTO> items
        );

    public record OrderItemDTO(
        Guid productId,
        string productName,
        int quantity,
        decimal unitPrice,
        decimal lineTotal);

}
