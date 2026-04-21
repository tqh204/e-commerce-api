using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Features.Product.Queries.GetProductQuery
{
    public record VariantDTO(
    Guid variantId,
    string sku,
    string? size,
    string? color,
    string? material,
    decimal price,
    int inventory
);
    public record ProductDTO
    (
        Guid productId,
        string name,
        string? description,
        decimal price,
        int stockQuantity,
        int categoryId,
        string CategoryName,
        List<VariantDTO> variants
    );
}
