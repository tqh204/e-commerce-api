using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Common.Lalamove
{
    public record CreateShipmentRequest(
        string quotationId,
        ContactRequest sender,
        ContactRequest recipient,
        decimal? codAmount
    );

    public record ContactRequest(
        string name,
        string phone
    );
}
