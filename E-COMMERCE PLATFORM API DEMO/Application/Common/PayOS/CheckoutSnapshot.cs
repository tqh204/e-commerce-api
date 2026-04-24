using System;
using System.Collections.Generic;

namespace Application.Common.PayOS
{
    public record CheckoutSnapshot(
        Guid userId,
        Guid? couponId,
        string? couponCode,
        decimal subTotal,
        decimal promotionDiscount,
        decimal couponDiscount,
        decimal rankDiscount,
        decimal totalAmount,
        IReadOnlyList<CheckoutSnapshotItem> items
    );

    public record CheckoutSnapshotItem(
        Guid productId,
        string productName,
        int quantity,
        decimal unitPrice,
        decimal lineTotal
    );
}
