using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Common.Lalamove
{
    public record ShippingFeeResponse(
      string quotationId,
      decimal fee,
      string currency,
      DateTime? expiresAt,
      string serviceType,
      string pickupStopId,
      string dropoffStopId
  );
}
