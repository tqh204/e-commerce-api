using System;

namespace Application.Common.Lalamove
{
    public record CreateShipmentResult(
        Guid shipmentId,
        string? providerOrderId,
        string? providerStatus
    );
}
