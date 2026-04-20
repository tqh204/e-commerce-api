using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces
{
    public interface IReviewRepository
    {
        Task<bool> HasCompletedOrderWithProductAsync(Guid userId, Guid productId); //Checking this userId whether bought completely productId in any order completed

        Task<bool> HasAlreadyReviewedAsync(Guid userId, Guid productId);//Preventing duplicate comment by check the comment of this user existed?
        Task AddAsync(Review review);
        Task<(IReadOnlyList<Review> items, int TotalCount)> GetByProductIdAsync(Guid productId, int page, int size);//Listing list review of this product by pagination
    }
}
