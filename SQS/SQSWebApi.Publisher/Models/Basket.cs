namespace SQSWebApi.Publisher.Models
{
    public sealed class Basket
    {
        public Guid Id { get; private set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public Basket()
        {
            Id = Guid.CreateVersion7();
        }
        public static List<Basket> GetAll()
        {
            var baskets = new List<Basket>
            {
                new Basket { ProductName = "Product 1", Quantity = 2, Price = 10.99m },
                new Basket { ProductName = "Product 2", Quantity = 1, Price = 5.49m },
                new Basket { ProductName = "Product 3", Quantity = 3, Price = 7.99m }
            };
            return baskets;
        }
    }

}
