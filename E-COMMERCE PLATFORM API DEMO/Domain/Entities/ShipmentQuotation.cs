using System;

namespace Domain.Entities
{
    public class ShipmentQuotation
    {
        public Guid shipmentQuotationId { get; set; }
        public string quotationId { get; set; } = null!;
        public decimal fee { get; set; }
        public string currency { get; set; } = "VND";
        public DateTime? expiresAt { get; set; }
        public string serviceType { get; set; } = null!;
        public string pickupStopId { get; set; } = null!;
        public string dropoffStopId { get; set; } = null!;

        public string pickupAddress { get; set; } = null!;
        public string pickupLat { get; set; } = null!;
        public string pickupLng { get; set; } = null!;

        public string dropoffAddress { get; set; } = null!;
        public string dropoffLat { get; set; } = null!;
        public string dropoffLng { get; set; } = null!;

        public decimal packageWeight { get; set; }
        public decimal? packageLength { get; set; }
        public decimal? packageWidth { get; set; }
        public decimal? packageHeight { get; set; }
        public string? packageQuantity { get; set; }
        public string? packageCategory { get; set; }

        public DateTime createdAt { get; set; } = DateTime.UtcNow;
    }
}
