using System.IO;
using System.Net;
using System.Threading.Tasks;
using AppFunctions.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace AppFunctions
{
    public class OnPaymentReceived
    {
        private readonly IConfiguration _configuration;

        public OnPaymentReceived(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [Function("OnPaymentReceived")]
        //[TableOutput()] it's removed from Microsoft.Azure.Functions.Worker.Extensions.Storage 5.* version
        public async Task<OrderOutputType> Run([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequestData req,
            FunctionContext executionContext)
        {
            var logger = executionContext.GetLogger("OnPaymentReceived");
            logger.LogInformation("Received a payment.");

            // if you need to get a secret from the Azure Key Vault
            // string secret = _configuration["YourSecretFromAzureKeyVault"];

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
