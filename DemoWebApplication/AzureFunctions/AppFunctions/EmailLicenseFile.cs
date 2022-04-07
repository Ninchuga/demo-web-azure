using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppFunctions.Models;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using SendGrid.Helpers.Mail;

namespace AppFunctions
{
    public static class EmailLicenseFile
    {
        [Function("EmailLicenseFile")]
        [SendGridOutput(ApiKey = "SendGridApiKey")]
        public static SendGridMessage Run([BlobTrigger("licenses/{id}.lic", Connection = "AzureWebJobsStorage")] string licenseFileContents,
            //[CosmosDBInput(databaseName: "OrdersDb", collectionName: "Orders", ConnectionStringSetting = "CosmosConnection", SqlQuery = "select * from Orders o where o.OrderId = {orderId}")] IEnumerable<Order> orders,
            [CosmosDBInput(databaseName: "OrdersDb", collectionName: "Orders", ConnectionStringSetting = "CosmosConnection", Id = "{id}")] Order order,
            string id,
            FunctionContext context)
        {
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

            SendGridMessage message = new();
            message.From = new EmailAddress(Environment.GetEnvironmentVariable("EmailSender"));
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
