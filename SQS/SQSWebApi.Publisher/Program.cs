using SQS.Shared;
using SQS.Shared.Messaging;
using SQSWebApi.Publisher.Messaging;
using SQSWebApi.Publisher.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddSharedServices(builder.Configuration);
builder.Services.AddSingleton<Publisher>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}




app.MapPost("/api/orders", async (Publisher publisher, CancellationToken cancellationToken) =>
{
    var baskets = Basket.GetAll();
    var orders = new List<Order>();
    var code = "ORD-" + Guid.NewGuid().ToString().ToUpper();
    foreach (var basket in baskets)
    {
        var order = new Order
        {
            Code = code,
            ProductName = basket.ProductName,
            Quantity = basket.Quantity,
            Price = basket.Price
        };
        orders.Add(order);
    }
    await publisher.SendMessageAsync(orders, QueueNames.Customers, nameof(Order), cancellationToken);

    return code;
})
.WithName("CreateOrders");


app.Run();



