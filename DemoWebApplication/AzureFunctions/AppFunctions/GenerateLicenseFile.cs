using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using AppFunctions.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace AppFunctions
{
    public static class GenerateLicenseFile
    {
        [Function("GenerateLicenseFile")]
        [BlobOutput(blobPath: "licenses/{id}.lic")] // specified blob name (licenses) and file name: ({rand-guid}.lic for random generated guid on insert)
        public static string Run([QueueTrigger("orders", Connection = "AzureWebJobsStorage")] Order order,
            FunctionContext context)
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.AppendLine($"OrderId: {order.id}");
            stringBuilder.AppendLine($"Email: {order.Email}");
            stringBuilder.AppendLine($"ProductId: {order.ProductId}");
            stringBuilder.AppendLine($"PurchaseDate: {DateTime.UtcNow}");
            var md5 = MD5.Create();
            var hash = md5.ComputeHash(Encoding.UTF8.GetBytes(order.Email + "secret"));
            stringBuilder.AppendLine($"SecretCode: {BitConverter.ToString(hash).Replace("-", "")}");

            return stringBuilder.ToString();
        }
    }
}
