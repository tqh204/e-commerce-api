using Application.Common.Results;
using Application.Interfaces;
using Domain.Entities;
using Domain.Enums;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Features.Order.Commands
{
    public class CheckoutCommandHandler : IRequestHandler<CheckoutCommand, Result<Guid>>
    {
        private readonly ICheckoutSnapshotService _checkoutSnapshotService;
        private readonly ICheckoutExecutionService _checkoutExecutionService;

        public CheckoutCommandHandler(
            ICheckoutSnapshotService checkoutSnapshotService,
            ICheckoutExecutionService checkoutExecutionService)
        {
            _checkoutSnapshotService = checkoutSnapshotService;
            _checkoutExecutionService = checkoutExecutionService;
        }

        public async Task<Result<Guid>> Handle(CheckoutCommand request, CancellationToken cancellationToken)
        {
            var snapshotResult = await _checkoutSnapshotService.BuildAsync(request.userId, cancellationToken);
            if (!snapshotResult.IsSuccess || snapshotResult.Data == null)
            {
                return Result<Guid>.Failure(snapshotResult.ErrorMessage);
            }
            return await _checkoutExecutionService.ExecuteAsync(request.userId, snapshotResult.Data, cancellationToken);
        }
    }
}
