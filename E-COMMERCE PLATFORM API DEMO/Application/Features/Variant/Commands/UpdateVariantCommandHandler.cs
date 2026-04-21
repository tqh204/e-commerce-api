using Application.Interfaces;
using Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
namespace Application.Features.Variant.Commands
{
    public class UpdateVariantCommandHandler : IRequestHandler<UpdateVariantCommand, Result<bool>>
    {
        private readonly IVariantRepository _variantRepository;
        private readonly IProductRepository _productRepository;
        private readonly IUnitOfWork _unitOfWork;
        public UpdateVariantCommandHandler(
            IVariantRepository variantRepository,
            IProductRepository productRepository,
            IUnitOfWork unitOfWork)
        {
            _variantRepository = variantRepository;
            _productRepository = productRepository;
            _unitOfWork = unitOfWork;
        }
        public async Task<Result<bool>> Handle(UpdateVariantCommand request, CancellationToken cancellationToken)
        {
            var variant = await _variantRepository.GetByIdAsync(request.variantId);
            if (variant == null)
                return Result<bool>.Failure("Không tìm thấy biến thể.");
            var duplicatedSku = await _variantRepository.ExistsSkuAsync(request.sku, request.variantId);
            if (duplicatedSku)
                return Result<bool>.Failure("SKU đã tồn tại.");
            var product = await _productRepository.GetProductByIdAsync(variant.productId);
            if (product == null)
                return Result<bool>.Failure("Không tìm thấy sản phẩm.");
            var allocatedExcludingCurrent = await _variantRepository.GetAllocatedInventoryAsync(variant.productId, variant.variantId);

            if (allocatedExcludingCurrent + request.inventory > product.stockQuantity)
                return Result<bool>.Failure("Tổng tồn kho biến thể vượt quá stock của sản phẩm.");
            var oldInventory = variant.inventory;
            var newInventory = request.inventory;
            variant.sku = request.sku.Trim();
            variant.size = request.size;
            variant.color = request.color;
            variant.material = request.material;
            variant.price = request.price;
            variant.inventory = newInventory;
            variant.updatedAt = DateTime.UtcNow;
            
            product.updatedAt = DateTime.UtcNow;
            _variantRepository.Update(variant);
            _productRepository.Update(product);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result<bool>.Success(true);
        }
    }
}
