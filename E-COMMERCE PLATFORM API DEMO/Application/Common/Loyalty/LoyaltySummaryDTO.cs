using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Common.Loyalty
{
    public record LoyaltySummaryDTO
    (
        Guid userId,
        int loyaltyPoints,
        string rank
    );
}
