namespace SQSWebApi.Publisher.Models
{
    public sealed class Order
    {

        public Guid Id { get; private set; }
        public string Code { get; init; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public Order()
        {
            Id = Guid.CreateVersion7();
        }

    }
}
