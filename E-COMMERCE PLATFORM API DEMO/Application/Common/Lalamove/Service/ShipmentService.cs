using Application.Interfaces;
using Domain.Entities;
using Domain.Enums;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Application.Common.Lalamove.Service
{
    public class ShipmentService : IShipmentService
    {
        private readonly ILalamoveClient _lalamoveClient;
        private readonly IOrderRepository _orderRepository;
        private readonly IShipmentRepository _shipmentRepository;
        private readonly IShipmentQuotationRepository _shipmentQuotationRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<ShipmentService> _logger;

        public ShipmentService(
            ILalamoveClient lalamoveClient,
            IOrderRepository orderRepository,
            IShipmentRepository shipmentRepository,
            IShipmentQuotationRepository shipmentQuotationRepository,
            IUnitOfWork unitOfWork,
            ILogger<ShipmentService> logger)
        {
            _lalamoveClient = lalamoveClient;
            _orderRepository = orderRepository;
            _shipmentRepository = shipmentRepository;
            _shipmentQuotationRepository = shipmentQuotationRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<Result<Guid>> CreateShipmentAsync(
             Guid orderId,
             CreateShipmentRequest request,
             CancellationToken cancellationToken = default)
        {
            var order = await _orderRepository.GetOrderIdAsync(orderId);
            if (order == null)
            {
                return Result<Guid>.Failure("Không tìm thấy order");
            }

            var existingShipment = await _shipmentRepository.GetByOrderIdAsync(orderId);
            if (existingShipment != null && !string.IsNullOrWhiteSpace(existingShipment.providerOrderId))
            {
                return Result<Guid>.Failure("Order này đã có shipment");
            }

            try
            {
                var quotation = await _shipmentQuotationRepository.GetByQuotationIdAsync(request.quotationId);
                if (quotation == null)
                {
                    return Result<Guid>.Failure("Không tìm thấy quotation đã chọn");
                }

                if (quotation.expiresAt.HasValue && quotation.expiresAt.Value <= DateTime.Now)
                {
                    return Result<Guid>.Failure("Quotation đã hết hạn, vui lòng tính phí lại");
                }

                var providerOrder = await _lalamoveClient.CreateOrderAsync(
                    request,
                    quotation.pickupStopId,
                    quotation.dropoffStopId,
                    cancellationToken);

                var shipment = existingShipment ?? new Shipment
                {
                    shipmentId = Guid.NewGuid(),
                    orderId = orderId,
                    createdAt = DateTime.UtcNow
                };

                shipment.provider = "LALAMOVE";
                shipment.status = MapShipmentStatus(
                    providerOrder.status,
                    eventType: null,
                    fallbackStatus: ShipmentStatus.OrderCreated);
                shipment.serviceType = quotation.serviceType;
                shipment.fee = quotation.fee;
                shipment.currency = quotation.currency;
                shipment.quotationId = quotation.quotationId;
                shipment.quotationExpiresAt = quotation.expiresAt;
                shipment.providerOrderId = providerOrder.providerOrderId;
                shipment.shareLink = providerOrder.shareLink;
                shipment.pickupAddress = quotation.pickupAddress;
                shipment.pickupLat = quotation.pickupLat;
                shipment.pickupLng = quotation.pickupLng;
                shipment.dropoffAddress = quotation.dropoffAddress;
                shipment.dropoffLat = quotation.dropoffLat;
                shipment.dropoffLng = quotation.dropoffLng;
                shipment.senderName = request.sender.name;
                shipment.senderPhone = request.sender.phone;
                shipment.recipientName = request.recipient.name;
                shipment.recipientPhone = request.recipient.phone;
                shipment.codAmount = request.codAmount;
                shipment.updatedAt = DateTime.UtcNow;

                if (existingShipment == null)
                {
                    await _shipmentRepository.AddAsync(shipment);
                }
                else
                {
                    _shipmentRepository.Update(shipment);
                }

                await _unitOfWork.SaveChangesAsync(cancellationToken);
                return Result<Guid>.Success(shipment.shipmentId);
            }
            catch (Exception ex)
            {
                return Result<Guid>.Failure($"Không thể tạo shipment: {ex.Message}");
            }
        }

        public async Task<Result<ShippingFeeResponse>> GetShippingFeeAsync(
           GetShippingFeeRequest request,
           CancellationToken cancellationToken = default)
        {
            try
            {
                var result = await _lalamoveClient.GetQuotationAsync(request, cancellationToken);
                var existingQuotation = await _shipmentQuotationRepository.GetByQuotationIdAsync(result.quotationId);

                if (existingQuotation == null)
                {
                    await _shipmentQuotationRepository.AddAsync(new ShipmentQuotation
                    {
                        shipmentQuotationId = Guid.NewGuid(),
                        quotationId = result.quotationId,
                        fee = result.fee,
                        currency = result.currency,
                        expiresAt = result.expiresAt,
                        serviceType = result.serviceType,
                        pickupStopId = result.pickupStopId,
                        dropoffStopId = result.dropoffStopId,
                        pickupAddress = request.pickup.address,
                        pickupLat = request.pickup.lat,
                        pickupLng = request.pickup.lng,
                        dropoffAddress = request.dropoff.address,
                        dropoffLat = request.dropoff.lat,
                        dropoffLng = request.dropoff.lng,
                        packageWeight = request.package.weight,
                        packageLength = request.package.length,
                        packageWidth = request.package.width,
                        packageHeight = request.package.height,
                        packageQuantity = request.package.quantity,
                        packageCategory = request.package.category,
                        createdAt = DateTime.UtcNow
                    });

                    await _unitOfWork.SaveChangesAsync(cancellationToken);
                }

                return Result<ShippingFeeResponse>.Success(result);
            }
            catch (Exception ex)
            {
                return Result<ShippingFeeResponse>.Failure($"Không thể lấy phí vận chuyển: {ex.Message}");
            }
        }

        public async Task<Result<bool>> HandleWebhookAsync(
            string rawPayload,
            CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation("Received Lalamove webhook payload: {RawPayload}", rawPayload);

                using var doc = JsonDocument.Parse(rawPayload);
                var root = doc.RootElement;

                var providerOrderId = root
                    .GetProperty("data")
                    .GetProperty("orderId")
                    .GetString();

                if (string.IsNullOrWhiteSpace(providerOrderId))
                {
                    return Result<bool>.Failure("Webhook không có provider order id");
                }

                var shipment = await _shipmentRepository.GetByProviderOrderIdAsync(providerOrderId);
                if (shipment == null)
                {
                    return Result<bool>.Failure("Không tìm thấy shipment");
                }

                var eventType = root.TryGetProperty("eventType", out var eventTypeElement)
                    ? eventTypeElement.GetString()
                    : null;

                var data = root.GetProperty("data");
                var providerStatus = data.TryGetProperty("status", out var statusElement)
                    ? statusElement.GetString()
                    : null;

                shipment.status = MapShipmentStatus(
                    providerStatus,
                    eventType,
                    shipment.status);
                shipment.driverId = ReadDriverId(data) ?? shipment.driverId;
                shipment.lastWebhookPayload = rawPayload;
                shipment.lastWebhookEvent = eventType;
                shipment.lastWebhookAt = DateTime.UtcNow;
                shipment.updatedAt = DateTime.UtcNow;

                _shipmentRepository.Update(shipment);

                var order = await _orderRepository.GetOrderIdAsync(shipment.orderId);
                if (order != null)
                {
                    if (shipment.status == ShipmentStatus.Delivered)
                    {
                        order.status = OrderStatus.Completed;
                        order.updatedAt = DateTime.UtcNow;
                        _orderRepository.Update(order);
                    }
                    else if (shipment.status is ShipmentStatus.Cancelled or ShipmentStatus.Failed)
                    {
                        order.status = OrderStatus.Cancelled;
                        order.updatedAt = DateTime.UtcNow;
                        _orderRepository.Update(order);
                    }
                }

                await _unitOfWork.SaveChangesAsync(cancellationToken);

                return Result<bool>.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to process Lalamove webhook payload: {RawPayload}", rawPayload);
                return Result<bool>.Failure($"Webhook xử lý thất bại: {ex.Message}");
            }
        }

        private static ShipmentStatus MapShipmentStatus(
            string? providerStatus,
            string? eventType,
            ShipmentStatus fallbackStatus)
        {
            if (string.Equals(eventType, "DRIVER_ASSIGNED", StringComparison.OrdinalIgnoreCase))
            {
                return ShipmentStatus.DriverAssigned;
            }

            return providerStatus?.ToUpperInvariant() switch
            {
                "ASSIGNING_DRIVER" => ShipmentStatus.OrderCreated,
                "ON_GOING" => ShipmentStatus.OnGoing,
                "PICKED_UP" => ShipmentStatus.OnGoing,
                "COMPLETED" => ShipmentStatus.Delivered,
                "CANCELED" => ShipmentStatus.Cancelled,
                "REJECTED" => ShipmentStatus.Failed,
                "EXPIRED" => ShipmentStatus.Failed,
                _ => fallbackStatus
            };
        }

        private static string? ReadDriverId(JsonElement data)
        {
            if (data.TryGetProperty("driverId", out var driverIdElement))
            {
                return driverIdElement.GetString();
            }

            if (data.TryGetProperty("driver", out var driverElement) &&
                driverElement.TryGetProperty("driverId", out var nestedDriverIdElement))
            {
                return nestedDriverIdElement.GetString();
            }

            return null;
        }
    }
}
