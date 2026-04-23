using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Application.Common.Lalamove
{
    public static class LalamoveAuth
    {
        public static string CreateSignature(
            string apiSecret,
            string timestampMs,
            string method,
            string path,
            string bodyJson)
        {
            var rawSignature = $"{timestampMs}\r\n{method}\r\n{path}\r\n\r\n{bodyJson}";

            var keyBytes = Encoding.UTF8.GetBytes(apiSecret);
            var dataBytes = Encoding.UTF8.GetBytes(rawSignature);

            using var hmac = new HMACSHA256(keyBytes);
            var hashBytes = hmac.ComputeHash(dataBytes);

            return Convert.ToHexString(hashBytes).ToLowerInvariant();
        }

        public static string CreateAuthToken(
            string apiKey,
            string timestampMs,
            string signature)
        {
            return $"{apiKey}:{timestampMs}:{signature}";
        }
    }
}
