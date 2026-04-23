using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Common.Lalamove
{
    public record GetShippingFeeRequest(
       string serviceType,
       string language,
       StopRequest pickup,
       StopRequest dropoff,
       PackageRequest package,
       List<string>? specialRequests = null,
       string? scheduleAt = null
   );

    public record StopRequest(
        string address,
        string lat,
        string lng
    );

    public record PackageRequest(
        decimal weight,
        decimal? length,
        decimal? width,
        decimal? height,
        string? quantity,
        string? category
    );
}
