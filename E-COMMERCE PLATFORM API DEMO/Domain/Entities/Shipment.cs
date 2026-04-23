using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entities
{
    public class Shipment
    {
        public Guid shipmentId { get; set; }
        public Guid orderId { get; set; }

        public string provider { get; set; } = "LALAMOVE";
        public ShipmentStatus status { get; set; } = ShipmentStatus.Pending;

        public string? serviceType { get; set; } 
        public decimal fee { get; set; }
        public string currency { get; set; } = "VND";

        public string? quotationId { get; set; } 
        public DateTime? quotationExpiresAt { get; set; }

        public string? providerOrderId { get; set; }
        public string? driverId { get; set; }
        public string? shareLink { get; set; }

        public string? pickupAddress { get; set; }
        public string? pickupLat { get; set; }
        public string? pickupLng { get; set; }

        public string? dropoffAddress { get; set; }
        public string? dropoffLat { get; set; }
        public string? dropoffLng { get; set; }

        public string? senderName { get; set; }
        public string? senderPhone { get; set; }
        public string? recipientName { get; set; }
        public string? recipientPhone { get; set; }

        public decimal? codAmount { get; set; }

        public string? lastWebhookEvent { get; set; }
        public string? lastWebhookPayload { get; set; }
        public DateTime? lastWebhookAt { get; set; }

        public string? failureReason { get; set; }

        public DateTime createdAt { get; set; } = DateTime.UtcNow;
        public DateTime? updatedAt { get; set; }

        public Order? order { get; set; }
    }
}
