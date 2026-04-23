using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Enums
{
    public enum ShipmentStatus
    {
        Pending = 0,
        QuoteReceived = 1,
        OrderCreated = 2,
        DriverAssigned = 3,
        OnGoing = 4,
        Delivered = 5,
        Cancelled = 6,
        Failed = 7
    }
}
