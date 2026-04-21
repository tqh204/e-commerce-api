using Application.Interfaces;
using Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Features.Variant.Commands
{
    public class CreateVariantCommandHandler : IRequestHandler<CreateVariantCommand, Result<Guid>>
    {
        private readonly IVariantRepository _variantRepository;
        private readonly IProductRepository _productRepository;
        private readonly IUnitOfWork _unitOfWork;
        public CreateVariantCommandHandler(
            IVariantRepository variantRepository,
            IProductRepository productRepository,
            IUnitOfWork unitOfWork)
        {
            _variantRepository = variantRepository;
            _productRepository = productRepository;
            _unitOfWork = unitOfWork;
        }
        public async Task<Result<Guid>> Handle(CreateVariantCommand request, CancellationToken cancellationToken)
        {
            if (!await _variantRepository.ProductExistsAsync(request.productId))
                return Result<Guid>.Failure("Sản phẩm không tồn tại.");
            if (await _variantRepository.ExistsSkuAsync(request.sku))
                return Result<Guid>.Failure("SKU đã tồn tại.");
            var product = await _productRepository.GetProductByIdAsync(request.productId);
            if (product == null)
                return Result<Guid>.Failure("Không tìm thấy sản phẩm.");
            var allocated = await _variantRepository.GetAllocatedInventoryAsync(request.productId);
            if (allocated + request.inventory > product.stockQuantity)
                return Result<Guid>.Failure("Tổng tồn kho biến thể vượt quá stock của sản phẩm.");
            var variant = new Domain.Entities.Variant
            {
                variantId = Guid.NewGuid(),
                productId = request.productId,
                sku = request.sku.Trim().ToUpper(),
                size = request.size,
                color = request.color,
                material = request.material,
                price = request.price,
                inventory = request.inventory
            };
            product.updatedAt = DateTime.UtcNow;
            _productRepository.Update(product);
            await _variantRepository.AddAsync(variant);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result<Guid>.Success(variant.variantId);
        }
    }
}
