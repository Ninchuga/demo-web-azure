using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;

namespace AppFunctions.Models
{
    public class Order
    {
        //public string OrderId { get; set; }
        public string id { get; set; } // this has to be in lower case in order to override auto-generated id property in Cosmos Db
        public string ProductId { get; set; }
        public string Email { get; set; }
        public decimal Price { get; set; }
    }
}
