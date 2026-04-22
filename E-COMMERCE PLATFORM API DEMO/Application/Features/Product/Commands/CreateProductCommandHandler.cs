using Application.Features.Product.Queries.GetProductByIdQuery;
using Application.Features.Product.Queries.GetProductQuery;
using Domain.Entities;
using Application.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;
using Application.Common.Results;

namespace Application.Features.Product.Commands
{
    public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, Result<Guid>>
    {
        private readonly IProductRepository _repository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IUnitOfWork _unitOfWork;
        public CreateProductCommandHandler(IProductRepository productRepository, ICategoryRepository categoryRepository, IUnitOfWork unitOfWork)
        {
            _repository = productRepository;
            _categoryRepository = categoryRepository;
            _unitOfWork = unitOfWork;
        }
        public async Task<Result<Guid>> Handle(CreateProductCommand request, CancellationToken cancellationToken)
        {
            var isExist = await _categoryRepository.IsCategoryExsits(request.categoryId);

            if(!isExist)
            {
                return Result<Guid>.Failure("Không tìm th?y danh m?c h?p l?");
            }

            var product = new Domain.Entities.Product
            {
                productId = Guid.NewGuid(),
                productName = request.productName,
                description = request.productDescription,
                price = request.price,
                stockQuantity = request.stockQuantity,
                categoryId = request.categoryId,
                isDeleted = false,
                createdAt = DateTime.UtcNow,
                updatedAt = null
                
            };
            await _repository.AddAsync(product);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result<Guid>.Success(product.productId);
        }
    }
}

