using Application.Common.Lalamove;
using Application.Interfaces;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.Json;

namespace Infrastructure.Services
{
    public class LalamoveClient :   ILalamoveClient
    {
        private readonly HttpClient _httpClient;
        private readonly LalamoveOptions _options;

        public LalamoveClient(HttpClient httpClient, IOptions<LalamoveOptions> options)
        {
            _httpClient = httpClient;
            _options = options.Value;
        }

        public async Task<ShippingFeeResponse> GetQuotationAsync(
    GetShippingFeeRequest request,
    CancellationToken cancellationToken = default)
        {
            var path = "/v3/quotations";

            var payload = BuildQuotationPayload(request);
            var bodyJson = JsonSerializer.Serialize(payload);

            using var httpRequest = CreateSignedRequest(HttpMethod.Post, path, bodyJson);
            httpRequest.Content = new StringContent(bodyJson, Encoding.UTF8, "application/json");

            var response = await _httpClient.SendAsync(httpRequest, cancellationToken);
            var responseBody = await response.Content.ReadAsStringAsync(cancellationToken);
            EnsureSuccessStatusCode(response, responseBody, "quotation");

            using var doc = JsonDocument.Parse(responseBody);
            var data = doc.RootElement.GetProperty("data");

            var quotationId = data.GetProperty("quotationId").GetString() ?? "";
            var serviceType = data.GetProperty("serviceType").GetString() ?? request.serviceType;
            var stops = data.GetProperty("stops");
            var pickupStopId = stops[0].GetProperty("stopId").GetString() ?? "";
            var dropoffStopId = stops[1].GetProperty("stopId").GetString() ?? "";

            var price = data.GetProperty("priceBreakdown");
            var fee = ReadDecimal(price.GetProperty("total"));
            var currency = price.GetProperty("currency").GetString() ?? "VND";

            DateTime? expiresAt = null;
            if (data.TryGetProperty("expiresAt", out var expiresElement) &&
                expiresElement.ValueKind == JsonValueKind.String &&
                DateTime.TryParse(expiresElement.GetString(), out var parsedExpires))
            {
                expiresAt = parsedExpires;
            }

            return new ShippingFeeResponse(
                quotationId,
                fee,
                currency,
                expiresAt,
                serviceType,
                pickupStopId,
                dropoffStopId);
        }


        public async Task<LalamoveCreateOrderResult> CreateOrderAsync(
    CreateShipmentRequest request,
    string pickupStopId,
    string dropoffStopId,
    CancellationToken cancellationToken = default)
        {
            var path = "/v3/orders";

            var payload = BuildCreateOrderPayload(
                request,
                pickupStopId,
                dropoffStopId);
            var bodyJson = JsonSerializer.Serialize(payload);

            using var httpRequest = CreateSignedRequest(HttpMethod.Post, path, bodyJson);
            httpRequest.Content = new StringContent(bodyJson, Encoding.UTF8, "application/json");

            var response = await _httpClient.SendAsync(httpRequest, cancellationToken);
            var responseBody = await response.Content.ReadAsStringAsync(cancellationToken);
            EnsureSuccessStatusCode(response, responseBody, "create-order");

            using var doc = JsonDocument.Parse(responseBody);
            var data = doc.RootElement.GetProperty("data");

            var providerOrderId = data.GetProperty("orderId").GetString() ?? "";
            var shareLink = data.TryGetProperty("shareLink", out var shareLinkElement)
                ? shareLinkElement.GetString()
                : null;

            var status = data.TryGetProperty("status", out var statusElement)
                ? statusElement.GetString()
                : null;

            return new LalamoveCreateOrderResult(providerOrderId, shareLink, status);
        }


        public async Task<LalamoveOrderDetailResult> GetOrderDetailAsync(
    string providerOrderId,
    CancellationToken cancellationToken = default)
        {
            var path = $"/v3/orders/{providerOrderId}";

            using var httpRequest = CreateSignedRequest(HttpMethod.Get, path, "");
            var response = await _httpClient.SendAsync(httpRequest, cancellationToken);
            var responseBody = await response.Content.ReadAsStringAsync(cancellationToken);
            EnsureSuccessStatusCode(response, responseBody, "order-detail");

            using var doc = JsonDocument.Parse(responseBody);
            var data = doc.RootElement.GetProperty("data");

            var status = data.GetProperty("status").GetString() ?? "UNKNOWN";
            var shareLink = data.TryGetProperty("shareLink", out var shareLinkElement)
                ? shareLinkElement.GetString()
                : null;

            string? driverId = null;
            if (data.TryGetProperty("driver", out var driverElement) &&
                driverElement.TryGetProperty("driverId", out var driverIdElement))
            {
                driverId = driverIdElement.GetString();
            }

            return new LalamoveOrderDetailResult(
                providerOrderId,
                status,
                driverId,
                shareLink);
        }


        private HttpRequestMessage CreateSignedRequest(HttpMethod method, string path, string bodyJson)
        {
            var timestampMs = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString();
            var signature = LalamoveAuth.CreateSignature(
                _options.ApiSecret,
                timestampMs,
                method.Method,
                path,
                bodyJson);

            var token = LalamoveAuth.CreateAuthToken(_options.ApiKey, timestampMs, signature);

            var request = new HttpRequestMessage(method, path);
            request.Headers.TryAddWithoutValidation("Authorization", $"hmac {token}");
            request.Headers.TryAddWithoutValidation("Market", _options.Market);
            request.Headers.TryAddWithoutValidation("Request-ID", Guid.NewGuid().ToString());
            return request;
        }

        private object BuildQuotationPayload(GetShippingFeeRequest request)
        {
            var data = new Dictionary<string, object?>
            {
                ["serviceType"] = request.serviceType,
                ["language"] = request.language,
                ["stops"] = new[]
                {
                    new
                    {
                        coordinates = new
                        {
                            lat = request.pickup.lat,
                            lng = request.pickup.lng
                        },
                        address = request.pickup.address
                    },
                    new
                    {
                        coordinates = new
                        {
                            lat = request.dropoff.lat,
                            lng = request.dropoff.lng
                        },
                        address = request.dropoff.address
                    }
                }
            };

            if (request.specialRequests is { Count: > 0 })
            {
                data["specialRequests"] = request.specialRequests;
            }

            if (!string.IsNullOrWhiteSpace(request.scheduleAt))
            {
                data["scheduleAt"] = request.scheduleAt;
            }

            var item = BuildItemPayload(request.package);
            if (item != null)
            {
                data["item"] = item;
            }

            return new
            {
                data
            };
        }

        private object BuildCreateOrderPayload(
            CreateShipmentRequest request,
            string pickupStopId,
            string dropoffStopId)
        {
            return new
            {
                data = new
                {
                    quotationId = request.quotationId,
                    sender = new
                    {
                        stopId = pickupStopId,
                        name = request.sender.name,
                        phone = request.sender.phone
                    },
                    recipients = new[]
                    {
                        new
                        {
                            stopId = dropoffStopId,
                            name = request.recipient.name,
                            phone = request.recipient.phone
                        }
                    }
                }
            };
        }

        private static object? BuildItemPayload(PackageRequest package)
        {
            var item = new Dictionary<string, object?>();

            if (package.weight > 0)
            {
                item["weight"] = MapWeightEnum(package.weight);
            }

            if (!string.IsNullOrWhiteSpace(package.quantity))
            {
                item["quantity"] = package.quantity;
            }

            if (!string.IsNullOrWhiteSpace(package.category))
            {
                item["categories"] = new[] { package.category };
            }

            return item.Count > 0 ? item : null;
        }

        private static string MapWeightEnum(decimal weight)
        {
            if (weight < 3) return "LESS_THAN_3_KG";
            if (weight <= 10) return "BETWEEN_3_TO_10_KG";
            if (weight <= 20) return "BETWEEN_10_TO_20_KG";
            return "ABOVE_20_KG";
        }

        private static void EnsureSuccessStatusCode(
            HttpResponseMessage response,
            string responseBody,
            string operation)
        {
            if (response.IsSuccessStatusCode)
            {
                return;
            }

            throw new HttpRequestException(
                $"Lalamove {operation} failed with status {(int)response.StatusCode}: {responseBody}",
                null,
                response.StatusCode);
        }

        private static decimal ReadDecimal(JsonElement element)
        {
            return element.ValueKind switch
            {
                JsonValueKind.Number => element.GetDecimal(),
                JsonValueKind.String when decimal.TryParse(
                    element.GetString(),
                    NumberStyles.Any,
                    CultureInfo.InvariantCulture,
                    out var parsed) => parsed,
                _ => throw new InvalidOperationException(
                    $"Cannot parse decimal from JSON value kind '{element.ValueKind}'.")
            };
        }

    }
}

