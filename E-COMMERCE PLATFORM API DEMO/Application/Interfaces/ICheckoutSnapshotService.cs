using Application.Common.PayOS;
using Application.Common.Results;
using System;

namespace Application.Interfaces
{
    public interface ICheckoutSnapshotService
    {
        Task<Result<CheckoutSnapshot>> BuildAsync(
            Guid userId,
            CancellationToken cancellationToken = default);
    }
}
