using Application.Interfaces;
using Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using Application.Common.Results;

namespace Application.Features.Review.Commands
{
    public class CreateReviewCommandHandler : IRequestHandler<CreateReviewCommand, Result<Guid>>
    {
        private readonly IReviewRepository _reviewRepository;
        private readonly IUnitOfWork _unitOfWork;
        public CreateReviewCommandHandler(IReviewRepository reviewRepository, IUnitOfWork unitOfWork)
        {
            _reviewRepository = reviewRepository;
            _unitOfWork = unitOfWork;
        }
        public async Task<Result<Guid>> Handle(CreateReviewCommand request, CancellationToken cancellationToken)
        {
            
            if (request.rating < 1 || request.rating > 5)
                return Result<Guid>.Failure("Rating ph?i t? 1 d?n 5 sao");
            
            var hasPurchased = await _reviewRepository.HasCompletedOrderWithProductAsync(
                request.userId, request.productId);
            if (!hasPurchased)
                return Result<Guid>.Failure("B?n ch? có th? dánh giá s?n ph?m dã mua và nh?n hàng thành công");
            
            var alreadyReviewed = await _reviewRepository.HasAlreadyReviewedAsync(
                request.userId, request.productId);
            if (alreadyReviewed)
                return Result<Guid>.Failure("B?n dã dánh giá s?n ph?m này r?i");
            
            var review = new Domain.Entities.Review
            {
                reviewId = Guid.NewGuid(),
                userId = request.userId,
                productId = request.productId,
                rating = request.rating,
                comment = request.comment,
                createdAt = DateTime.UtcNow
            };
            await _reviewRepository.AddAsync(review);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result<Guid>.Success(review.reviewId);
        }
    }
}

