using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using AppFunctions.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace AppFunctions
{
    public static class OnPaymentReceived
    {
        [Function("OnPaymentReceived")]
        //[TableOutput()] it's removed from Microsoft.Azure.Functions.Worker.Extensions.Storage 5.* version
        public static async Task<OrderOutputType> Run([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequestData req,
            FunctionContext executionContext)
        {
            var logger = executionContext.GetLogger("OnPaymentReceived");
            logger.LogInformation("Received a payment.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            Order order = JsonConvert.DeserializeObject<Order>(requestBody);

            logger.LogInformation($"Order {order.id} received from {order.Email} for product {order.ProductId}");

            return new OrderOutputType
            {
                OrderQueueOutput = order,
                OrderCosmosOutput = order,
                UserResponse = req.CreateResponse(HttpStatusCode.OK)
            };
        }
    }

    public class OrderOutputType
    {
        [QueueOutput(queueName: "orders")] // store message (Order) to queue with QueueOutput binding
        public Order OrderQueueOutput { get; set; }

        [CosmosDBOutput(databaseName: "OrdersDb", collectionName: "Orders", ConnectionStringSetting = "CosmosConnection", CreateIfNotExists = true)]
        public Order OrderCosmosOutput { get; set; }

        public HttpResponseData UserResponse { get; set; }
    }
}
