using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Features.Cart
{
    public record CartDTO
    (
        Guid cartId,
        Guid userId,
        IReadOnlyList<CartItemDTO> items,
        decimal totalAmount,
        string? couponCode,
        decimal discountAmount,
        decimal finalAmount
    );

    public record CartItemDTO(
        Guid cartItemId,
        Guid productId,
        string productName,
        decimal unitPrice,
        int quantity,
        decimal lineTotal
        );
}
