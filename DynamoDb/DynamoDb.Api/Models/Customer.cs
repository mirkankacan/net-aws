using System.Text.Json.Serialization;

namespace DynamoDb.Api.Models
{
    public sealed class Customer
    {
        [JsonPropertyName("pk")]
        public string Pk => Id.ToString();
        [JsonPropertyName("sk")]
        public string Sk => Id.ToString();
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string Email { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public Customer()
        {
            Id = Guid.NewGuid();
        }

        public Customer(Guid id)
        {
            Id = id;
        }
    }
}
