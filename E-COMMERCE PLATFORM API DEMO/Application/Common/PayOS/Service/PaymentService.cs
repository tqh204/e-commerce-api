using Application.Common.Lalamove;
using Application.Common.Results;
using Application.Interfaces;
using Domain.Entities;
using Domain.Enums;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Application.Common.PayOS.Service
{
    public class PaymentService : IPaymentService
    {
        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        private readonly IPaymentRepository _paymentRepository;
        private readonly IUserRepository _userRepository;
        private readonly ICheckoutSnapshotService _checkoutSnapshotService;
        private readonly ICheckoutExecutionService _checkoutExecutionService;
        private readonly IPayOSClient _payOSClient;
        private readonly IShipmentService _shipmentService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<PaymentService> _logger;

        public PaymentService(
            IPaymentRepository paymentRepository,
            IUserRepository userRepository,
            ICheckoutSnapshotService checkoutSnapshotService,
            ICheckoutExecutionService checkoutExecutionService,
            IPayOSClient payOSClient,
            IShipmentService shipmentService,
            IUnitOfWork unitOfWork,
            ILogger<PaymentService> logger)
        {
            _paymentRepository = paymentRepository;
            _userRepository = userRepository;
            _checkoutSnapshotService = checkoutSnapshotService;
            _checkoutExecutionService = checkoutExecutionService;
            _payOSClient = payOSClient;
            _shipmentService = shipmentService;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<Result<CreatePayOSPaymentResponse>> CreatePaymentLinkAsync(
            Guid userId,
            CreatePayOSPaymentRequest request,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(request.returnUrl) || string.IsNullOrWhiteSpace(request.cancelUrl))
            {
                return Result<CreatePayOSPaymentResponse>.Failure("Thiếu returnUrl hoặc cancelUrl");
            }

            var user = await _userRepository.GetUserByIdAsync(userId);
            if (user == null)
            {
                return Result<CreatePayOSPaymentResponse>.Failure("Không tìm thấy user");
            }

            var snapshotResult = await _checkoutSnapshotService.BuildAsync(userId, cancellationToken);
            if (!snapshotResult.IsSuccess || snapshotResult.Data == null)
            {
                return Result<CreatePayOSPaymentResponse>.Failure(snapshotResult.ErrorMessage);
            }

            var orderCode = await GenerateOrderCodeAsync(cancellationToken);
            var description = BuildDescription(request.description, orderCode);

            var payment = new Payment
            {
                paymentId = Guid.NewGuid(),
                userId = userId,
                orderCode = orderCode,
                amount = snapshotResult.Data.totalAmount,
                description = description,
                status = PaymentStatus.Pending,
                providerStatus = PaymentStatus.Pending.ToString(),
                returnUrl = request.returnUrl,
                cancelUrl = request.cancelUrl,
                cartSnapshotJson = JsonSerializer.Serialize(snapshotResult.Data, JsonOptions),
                selectedShipmentJson = request.shipment == null
                    ? null
                    : JsonSerializer.Serialize(request.shipment, JsonOptions),
                createdAt = DateTime.UtcNow,
                updatedAt = DateTime.UtcNow
            };

            await _paymentRepository.AddAsync(payment);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var payOSResult = await _payOSClient.CreatePaymentLinkAsync(payment, snapshotResult.Data, user, cancellationToken);
            if (!payOSResult.IsSuccess || payOSResult.Data == null)
            {
                payment.status = PaymentStatus.Failed;
                payment.providerStatus = PaymentStatus.Failed.ToString();
                payment.failureReason = payOSResult.ErrorMessage;
                payment.updatedAt = DateTime.UtcNow;
                _paymentRepository.Update(payment);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                return Result<CreatePayOSPaymentResponse>.Failure(payOSResult.ErrorMessage);
            }

            payment.paymentLinkId = payOSResult.Data.paymentLinkId;
            payment.checkoutUrl = payOSResult.Data.checkoutUrl;
            payment.providerStatus = payOSResult.Data.providerStatus;
            payment.updatedAt = DateTime.UtcNow;
            _paymentRepository.Update(payment);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<CreatePayOSPaymentResponse>.Success(new CreatePayOSPaymentResponse(
                payment.paymentId,
                payment.orderCode,
                payment.checkoutUrl!,
                payment.status.ToString()));
        }

        public async Task<Result<PaymentStatusResponse>> GetPaymentStatusAsync(
            Guid userId,
            Guid paymentId,
            CancellationToken cancellationToken = default)
        {
            var payment = await _paymentRepository.GetByPaymentIdAsync(paymentId);
            if (payment == null || payment.userId != userId)
            {
                return Result<PaymentStatusResponse>.Failure("Không tìm thấy payment");
            }

            return Result<PaymentStatusResponse>.Success(new PaymentStatusResponse(
                payment.paymentId,
                payment.status.ToString(),
                payment.orderCode,
                payment.linkedOrderId,
                payment.linkedShipmentId,
                payment.checkoutUrl));
        }

        public async Task<Result<bool>> HandleWebhookAsync(string rawPayload, CancellationToken cancellationToken = default)
        {
            try
            {
                var verifiedResult = await _payOSClient.VerifyWebhookAsync(rawPayload, cancellationToken);
                if (!verifiedResult.IsSuccess || verifiedResult.Data == null)
                {
                    return Result<bool>.Failure(verifiedResult.ErrorMessage);
                }

                var payment = await _paymentRepository.GetByOrderCodeAsync(verifiedResult.Data.orderCode);
                if (payment == null)
                {
                    _logger.LogInformation(
                        "Bỏ qua webhook PayOS không khớp payment local. OrderCode: {OrderCode}",
                        verifiedResult.Data.orderCode);
                    return Result<bool>.Success(true);
                }

                var providerStatusResult = await _payOSClient.GetPaymentStatusAsync(payment.orderCode, cancellationToken);
                if (!providerStatusResult.IsSuccess || providerStatusResult.Data == null)
                {
                    return Result<bool>.Failure(providerStatusResult.ErrorMessage);
                }

                payment.providerStatus = providerStatusResult.Data.providerStatus;
                payment.providerReference = verifiedResult.Data.providerReference ?? providerStatusResult.Data.providerReference;
                payment.transactionDateTime = verifiedResult.Data.transactionDateTime ?? providerStatusResult.Data.transactionDateTime;
                payment.paymentLinkId ??= verifiedResult.Data.paymentLinkId ?? providerStatusResult.Data.paymentLinkId;
                payment.status = MapPaymentStatus(providerStatusResult.Data.providerStatus);
                payment.updatedAt = DateTime.UtcNow;

                if (payment.status == PaymentStatus.Paid && payment.paidAt == null)
                {
                    payment.paidAt = DateTime.UtcNow;
                }

                if (payment.status != PaymentStatus.Paid)
                {
                    _paymentRepository.Update(payment);
                    await _unitOfWork.SaveChangesAsync(cancellationToken);
                    return Result<bool>.Success(true);
                }

                if (payment.linkedOrderId.HasValue)
                {
                    _paymentRepository.Update(payment);
                    await _unitOfWork.SaveChangesAsync(cancellationToken);
                    return Result<bool>.Success(true);
                }

                var snapshot = JsonSerializer.Deserialize<CheckoutSnapshot>(payment.cartSnapshotJson, JsonOptions);
                if (snapshot == null)
                {
                    payment.failureReason = "Không thể đọc snapshot checkout từ payment";
                    _paymentRepository.Update(payment);
                    await _unitOfWork.SaveChangesAsync(cancellationToken);
                    return Result<bool>.Success(true);
                }

                var executeResult = await _checkoutExecutionService.ExecuteAsync(payment.userId, snapshot, cancellationToken);
                if (!executeResult.IsSuccess || executeResult.Data == Guid.Empty)
                {
                    payment.failureReason = executeResult.ErrorMessage;
                    _paymentRepository.Update(payment);
                    await _unitOfWork.SaveChangesAsync(cancellationToken);
                    return Result<bool>.Success(true);
                }

                payment.linkedOrderId = executeResult.Data;
                payment.failureReason = null;

                if (!string.IsNullOrWhiteSpace(payment.selectedShipmentJson))
                {
                    var shipmentRequest = JsonSerializer.Deserialize<CreateShipmentRequest>(payment.selectedShipmentJson, JsonOptions);
                    if (shipmentRequest != null)
                    {
                        var shipmentResult = await _shipmentService.CreateShipmentAsync(executeResult.Data, shipmentRequest, cancellationToken);
                        if (!shipmentResult.IsSuccess || shipmentResult.Data == null)
                        {
                            payment.failureReason = shipmentResult.ErrorMessage;
                        }
                        else
                        {
                            payment.linkedShipmentId = shipmentResult.Data.shipmentId;
                        }
                    }
                }

                _paymentRepository.Update(payment);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                return Result<bool>.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "PayOS webhook xử lý thất bại. Payload: {Payload}", rawPayload);
                return Result<bool>.Failure($"Webhook PayOS xử lý thất bại: {ex.Message}");
            }
        }

        private async Task<long> GenerateOrderCodeAsync(CancellationToken cancellationToken)
        {
            for (var attempt = 0; attempt < 10; attempt++)
            {
                var candidate = long.Parse($"{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}{Random.Shared.Next(100, 999)}");
                var existing = await _paymentRepository.GetByOrderCodeAsync(candidate);
                if (existing == null)
                {
                    return candidate;
                }
            }

            throw new InvalidOperationException("Không thể sinh orderCode duy nhất cho PayOS");
        }

        private static string BuildDescription(string? description, long orderCode)
        {
            var value = string.IsNullOrWhiteSpace(description)
                ? $"DH{orderCode}"
                : description.Trim();

            return value.Length <= 25 ? value : value[..25];
        }

        private static PaymentStatus MapPaymentStatus(string providerStatus)
        {
            return providerStatus.ToUpperInvariant() switch
            {
                "PAID" => PaymentStatus.Paid,
                "PROCESSING" => PaymentStatus.Processing,
                "PENDING" => PaymentStatus.Pending,
                "CANCELLED" => PaymentStatus.Cancelled,
                "CANCELED" => PaymentStatus.Cancelled,
                "EXPIRED" => PaymentStatus.Expired,
                _ => PaymentStatus.Failed
            };
        }
    }
}
