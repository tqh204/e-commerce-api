using Domain.Enums;
using System;

namespace Domain.Entities
{
    public class Payment
    {
        public Guid paymentId { get; set; }
        public Guid userId { get; set; }
        public long orderCode { get; set; }
        public decimal amount { get; set; }
        public string description { get; set; } = null!;
        public string? paymentLinkId { get; set; }
        public string? checkoutUrl { get; set; }
        public PaymentStatus status { get; set; } = PaymentStatus.Pending;
        public string? providerStatus { get; set; }
        public string? providerReference { get; set; }
        public DateTime? transactionDateTime { get; set; }
        public Guid? linkedOrderId { get; set; }
        public Guid? linkedShipmentId { get; set; }
        public string returnUrl { get; set; } = null!;
        public string cancelUrl { get; set; } = null!;
        public string cartSnapshotJson { get; set; } = null!;
        public string? selectedShipmentJson { get; set; }
        public string? failureReason { get; set; }
        public DateTime createdAt { get; set; } = DateTime.UtcNow;
        public DateTime updatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? paidAt { get; set; }

        public User? user { get; set; }
        public Order? linkedOrder { get; set; }
        public Shipment? linkedShipment { get; set; }
    }
}
