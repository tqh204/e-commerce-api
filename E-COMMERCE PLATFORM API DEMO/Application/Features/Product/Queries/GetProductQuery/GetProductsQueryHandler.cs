using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using MediatR;
using Domain.Interfaces;
namespace Application.Features.Product.Queries.GetProductQuery
{
    public class GetProductsQueryHandler : IRequestHandler<GetProductQuery, PagedResult<ProductDTO>>
    {
        private IProductRepository _productRepository;
        public GetProductsQueryHandler(IProductRepository productRepository) => _productRepository = productRepository;

        public async Task<PagedResult<ProductDTO>> Handle(GetProductQuery request, CancellationToken cancellationToken)
        {
            var (items, totalCount) = await _productRepository.GetPagedAsync(
                request.page,
                request.size,
                request.categoryId,
                request.minPrice,
                request.maxPrice);

            var productsDTO = items.Select(p => new ProductDTO(
                p.productId,
                p.productName,
                p.description,
                p.price,
                p.stockQuantity,
                p.categoryId,
                p.category.categoryName
            )).ToList();

            return new PagedResult<ProductDTO>
            {
                items = productsDTO,
                page = request.page,
                size = request.size,
                totalCount = totalCount
            };
        }
    }
}
