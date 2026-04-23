using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Common.Lalamove
{
    public record LalamoveOrderDetailResult(
        string providerOrderId,
        string status,
        string? driverId,
        string? shareLink
    );
}
