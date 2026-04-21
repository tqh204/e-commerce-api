using Application.Features.Product.Queries.GetProductQuery;
using Domain.Entities;
using Application.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Features.Product.Queries.GetProductByIdQuery
{
    public class GetProductByIdQueryHandler : IRequestHandler<GetProductByIdQuery, Result<ProductDTO>>
    {
        private readonly IProductRepository _productRepository;

        public GetProductByIdQueryHandler(IProductRepository productRepository) => _productRepository = productRepository;
        public async Task<Result<ProductDTO>> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
        {
            var product = await _productRepository.GetProductByIdAsync(request.productId);//Find product by productId   

            if (product == null)    
            {
                return Result<ProductDTO>.Failure("Không tìm thấy sản phẩm");
            }

            var productDTO = new ProductDTO(
                product.productId,
                product.productName,
                product.description,
                product.price,
                product.stockQuantity,
                product.categoryId,
                product.category.categoryName,
                product.variants
                        .Where(v => !v.isDeleted)
                        .Select(v => new VariantDTO(
                                v.variantId,
                                v.sku ?? string.Empty,
                                v.size,
                                v.color,
                                v.material,
                                v.price,
                                v.inventory
                                    ))
        .ToList());
            return Result<ProductDTO>.Success(productDTO);
        }
    }
}
