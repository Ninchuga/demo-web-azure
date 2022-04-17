using System;
using System.Text;
using System.Threading.Tasks;
using AppFunctions.Constants;
using AppFunctions.Models;
using AppFunctions.Services;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using SendGrid.Helpers.Mail;

namespace AppFunctions
{
    public class EmailLicenseFile
    {
        private readonly IKeyVaultManager _keyVaultManager;

        public EmailLicenseFile(IKeyVaultManager keyVaultManager)
        {
            _keyVaultManager = keyVaultManager;
        }

        [Function("EmailLicenseFile")]
        [SendGridOutput(ApiKey = AzureKeyVaultKeys.SendGridApiKey)]
        public async Task<SendGridMessage> Run([BlobTrigger("licenses/{id}.lic", Connection = "AzureWebJobsStorage")] string licenseFileContents,
            //[CosmosDBInput(databaseName: "OrdersDb", collectionName: "Orders", ConnectionStringSetting = "CosmosConnection", SqlQuery = "select * from Orders o where o.OrderId = {orderId}")] IEnumerable<Order> orders,
            [CosmosDBInput(databaseName: "OrdersDb", collectionName: "Orders", ConnectionStringSetting = "CosmosConnection", Id = "{id}")] Order order,
            string id,
            FunctionContext context)
        {
            var logger = context.GetLogger("OnPaymentReceived");
            logger.LogInformation("Sending email to customer {Email}", order.Email);

            // In case we need more complex query we can use Cosmos Client
            //string email = string.Empty;
            //string connectionString = Environment.GetEnvironmentVariable("CosmosConnection");
            //using (CosmosClient client = new CosmosClient(connectionString))
            //{
            //    var database = client.GetDatabase("OrdersDb");
            //    var container = database.GetContainer("Orders");

            //    string queryText = "select * from Orders o where o.id = @id";
            //    var queryDefinition = new QueryDefinition(queryText).WithParameter("@id", id);
            //    var feedIterator = container.GetItemQueryIterator<Order>(queryDefinition);
            //    while (feedIterator.HasMoreResults)
            //    {
            //        FeedResponse<Order> response = await feedIterator.ReadNextAsync();
            //        Order ord = response.First();
            //        email = ord.Email;
            //    }
            //}

            string senderEmail = await _keyVaultManager.GetSecret(AzureKeyVaultKeys.SenderEmail);

            SendGridMessage message = new();
            message.From = new EmailAddress(senderEmail);
            message.Subject = "Your license file";
            message.AddTo(order.Email);
            var plainTextBytes = Encoding.UTF8.GetBytes(licenseFileContents);
            var base64 = Convert.ToBase64String(plainTextBytes);
            message.AddAttachment(filename: id, base64Content: base64, type: "text/plain");
            message.HtmlContent = "Thank you for your order";

            return message;
        }
    }
}
