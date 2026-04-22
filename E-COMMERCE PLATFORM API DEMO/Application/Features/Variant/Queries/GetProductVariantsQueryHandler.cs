using Application.Features.Product.Queries.GetProductQuery;
using Application.Interfaces;
using Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using Application.Common.Results;

namespace Application.Features.Variant.Queries
{
    public class GetProductVariantsQueryHandler : IRequestHandler<GetProductVariantsQuery, PagedResult<VariantDTO>>
    {
        private readonly IVariantRepository _variantRepository;
        public GetProductVariantsQueryHandler(IVariantRepository variantRepository)
        {
            _variantRepository = variantRepository;
        }
        public async Task<PagedResult<VariantDTO>> Handle(GetProductVariantsQuery request, CancellationToken cancellationToken)
        {
            var (items, totalCount) = await _variantRepository.GetByProductIdPagedAsync(
                request.productId,
                request.page,
                request.size);
            var variantDtos = items.Select(v => new VariantDTO(
                v.variantId,
                v.sku ?? string.Empty,
                v.size,
                v.color,
                v.material,
                v.price,
                v.inventory
            )).ToList();
            return new PagedResult<VariantDTO>
            {
                items = variantDtos,
                page = request.page,
                size = request.size,
                totalCount = totalCount
            };
        }
    }
}

