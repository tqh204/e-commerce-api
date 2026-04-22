using Domain.Entities;
using Application.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using Application.Common.Results;

namespace Application.Features.Product.Commands
{
    public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand, Result<bool>>
    {
        private IProductRepository _productRepository;
        private ICategoryRepository _categoryRepository;
        private IUnitOfWork _unitOfWork;
        public UpdateProductCommandHandler(IProductRepository productRepository, ICategoryRepository categoryRepository, IUnitOfWork unitOfWork)
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<bool>> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
        {
            var product = await _productRepository.GetProductByIdAsync(request.productId);
            if(product == null)
            {
                return Result<bool>.Failure("Không tìm th?y product");
            }

            var categoryExists = await _categoryRepository.IsCategoryExsits(request.categoryId);
            if(!categoryExists)
            {
                return Result<bool>.Failure("Không tìm th?y category");
            }

            product.productName = request.productName;
            product.description = request.productDescription;
            product.price = request.price;
            product.stockQuantity = request.stockQuantity;
            product.categoryId = request.categoryId;
            product.updatedAt = DateTime.UtcNow;

             _productRepository.Update(product);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<bool>.Success(true);
        }
    }
}

