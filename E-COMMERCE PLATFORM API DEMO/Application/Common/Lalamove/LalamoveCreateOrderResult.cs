using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Common.Lalamove
{
    public record LalamoveCreateOrderResult(
        string providerOrderId,
        string? shareLink,
        string? status
    );
}
