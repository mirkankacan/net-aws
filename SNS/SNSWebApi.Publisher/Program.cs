using SNS.Shared;
using SNS.Shared.Messaging;
using SNSWebApi.Publisher.Messaging;
using SNSWebApi.Publisher.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddSharedServices(builder.Configuration);
builder.Services.AddSingleton<SnsPublisher>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapPost("/api/orders", async (SnsPublisher publisher, CancellationToken cancellationToken) =>
{
    var baskets = Basket.GetAll();
    var orders = new List<Order>();
    var singleOrders = new List<Order>();

    var code = "ORD-" + Guid.NewGuid().ToString().ToUpper();
    foreach (var basket in baskets)
    {
        if (basket.Quantity == 1)
        {
            var singleOrder = new Order
            {
                Code = code,
                ProductName = basket.ProductName,
                Quantity = basket.Quantity,
                Price = basket.Price
            };
            singleOrders.Add(singleOrder);
        }
        else
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

    }
    if (singleOrders.Any())
    {
        await publisher.PublishAsync(singleOrders, TopicNames.Customers, nameof(Order), true, cancellationToken);
    }

    if (orders.Any())
    {
        await publisher.PublishAsync(orders, TopicNames.Customers, nameof(Order), false, cancellationToken);
    }

    return code;
})
.WithName("CreateOrders");


app.Run();



