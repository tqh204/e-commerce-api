using Application.Common.PayOS;
using Application.Common.Results;
using Application.Interfaces;
using Domain.Entities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PayOS.Models.V2.PaymentRequests;
using PayOS.Models.Webhooks;
using System.Text.Json;
using PayOSSdkClient = PayOS.PayOSClient;
using PayOSSdkOptions = PayOS.PayOSOptions;

namespace Infrastructure.Services
{
    public class PayOSClientService : IPayOSClient
    {
        private readonly PayOSSettings _settings;
        private readonly ILogger<PayOSClientService> _logger;

        public PayOSClientService(
            IOptions<PayOSSettings> options,
            ILogger<PayOSClientService> logger)
        {
            _settings = options.Value;
            _logger = logger;
        }

        public async Task<Result<PayOSCreatePaymentLinkResult>> CreatePaymentLinkAsync(
            Payment payment,
            CheckoutSnapshot snapshot,
            User user,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var client = CreateClient();
                var request = new CreatePaymentLinkRequest
                {
                    OrderCode = payment.orderCode,
                    Amount = Convert.ToInt64(payment.amount),
                    Description = payment.description,
                    ReturnUrl = payment.returnUrl,
                    CancelUrl = payment.cancelUrl,
                    BuyerName = user.username,
                    BuyerEmail = user.email,
                    Items = snapshot.items.Select(item => new PaymentLinkItem
                    {
                        Name = item.productName,
                        Quantity = item.quantity,
                        Price = checked((int)item.unitPrice)
                    }).ToList()
                };

                var response = await client.PaymentRequests.CreateAsync(request);
                return Result<PayOSCreatePaymentLinkResult>.Success(new PayOSCreatePaymentLinkResult(
                    response.PaymentLinkId,
                    response.CheckoutUrl,
                    response.Status.ToString()));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "PayOS create payment link thất bại cho orderCode {OrderCode}", payment.orderCode);
                return Result<PayOSCreatePaymentLinkResult>.Failure($"Không thể tạo payment link PayOS: {ex.Message}");
            }
        }

        public async Task<Result<PayOSPaymentLinkStatusResult>> GetPaymentStatusAsync(
            long orderCode,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var client = CreateClient();
                var response = await client.PaymentRequests.GetAsync(orderCode);

                return Result<PayOSPaymentLinkStatusResult>.Success(new PayOSPaymentLinkStatusResult(
                    response.Status.ToString(),
                    response.Id.ToString(),
                    response.Transactions?.FirstOrDefault()?.Reference?.ToString(),
                    TryParseDateTime(response.Transactions?.FirstOrDefault()?.TransactionDateTime?.ToString())));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "PayOS lấy trạng thái thất bại cho orderCode {OrderCode}", orderCode);
                return Result<PayOSPaymentLinkStatusResult>.Failure($"Không thể lấy trạng thái payment PayOS: {ex.Message}");
            }
        }

        public async Task<Result<PayOSVerifiedWebhookResult>> VerifyWebhookAsync(
            string rawPayload,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var client = CreateClient();
                var webhook = JsonSerializer.Deserialize<Webhook>(rawPayload, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (webhook == null)
                {
                    return Result<PayOSVerifiedWebhookResult>.Failure("Payload webhook PayOS không hợp lệ");
                }

                var verified = await client.Webhooks.VerifyAsync(webhook);
                return Result<PayOSVerifiedWebhookResult>.Success(new PayOSVerifiedWebhookResult(
                    verified.OrderCode,
                    verified.PaymentLinkId.ToString(),
                    verified.Reference?.ToString(),
                    TryParseDateTime(verified.TransactionDateTime?.ToString())));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "PayOS verify webhook thất bại");
                return Result<PayOSVerifiedWebhookResult>.Failure($"Webhook PayOS không hợp lệ: {ex.Message}");
            }
        }

        private PayOSSdkClient CreateClient()
        {
            if (string.IsNullOrWhiteSpace(_settings.ClientId) ||
                string.IsNullOrWhiteSpace(_settings.ApiKey) ||
                string.IsNullOrWhiteSpace(_settings.ChecksumKey))
            {
                throw new InvalidOperationException("Thiếu cấu hình PayOS: ClientId/ApiKey/ChecksumKey");
            }

            return new PayOSSdkClient(new PayOSSdkOptions
            {
                ClientId = _settings.ClientId,
                ApiKey = _settings.ApiKey,
                ChecksumKey = _settings.ChecksumKey,
                PartnerCode = _settings.PartnerCode
            });
        }

        private static DateTime? TryParseDateTime(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return null;
            }

            return DateTime.TryParse(value, out var parsed)
                ? parsed
                : null;
        }
    }
}
