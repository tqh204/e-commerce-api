using Application.Interfaces;
using Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Features.Variant.Commands
{
    public class DeleteVariantCommandHandler : IRequestHandler<DeleteVariantCommand, Result<bool>>
    {
        private readonly IVariantRepository _variantRepository;
        private readonly IProductRepository _productRepository;
        private readonly IUnitOfWork _unitOfWork;
        public DeleteVariantCommandHandler(
            IVariantRepository variantRepository,
            IProductRepository productRepository,
            IUnitOfWork unitOfWork)
        {
            _variantRepository = variantRepository;
            _productRepository = productRepository;
            _unitOfWork = unitOfWork;
        }
        public async Task<Result<bool>> Handle(DeleteVariantCommand request, CancellationToken cancellationToken)
        {
            var variant = await _variantRepository.GetByIdAsync(request.variantId);
            if (variant == null)
                return Result<bool>.Failure("Không tìm thấy biến thể.");
            var product = await _productRepository.GetProductByIdAsync(variant.productId);
            if (product == null)
                return Result<bool>.Failure("Không tìm thấy sản phẩm.");
            var nextProductStock = product.stockQuantity - variant.inventory;
            if (nextProductStock < 0)
                return Result<bool>.Failure("Tổng tồn kho sản phẩm không hợp lệ.");
            product.updatedAt = DateTime.UtcNow;
            _variantRepository.SoftDelete(variant);
            _productRepository.Update(product);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result<bool>.Success(true);
        }
    }
}
