using System.Text.Json.Serialization;

namespace DynamoDb.Transaction
{
    public sealed class ShoppingCart
    {
        [JsonPropertyName("pk")]
        public string Pk => Id.ToString();
        [JsonPropertyName("sk")]
        public string Sk => Id.ToString();
        public Guid Id { get; set; }
        public string ProductName { get; set; }
    }
}
