using Application.Common.PayOS;
using Application.Common.Results;
using System;

namespace Application.Interfaces
{
    public interface ICheckoutExecutionService
    {
        Task<Result<Guid>> ExecuteAsync(
            Guid userId,
            CheckoutSnapshot snapshot,
            CancellationToken cancellationToken = default);
    }
}
