using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
using DynamoDb.Transaction;
using Microsoft.Extensions.Hosting;
using System.Text.Json;

var builder = Host.CreateApplicationBuilder(args);
using var host = builder.Build();

var client = new AmazonDynamoDBClient(builder.Configuration["AwsOptions:AccessKey"], builder.Configuration["AwsOptions:SecretKey"], Amazon.RegionEndpoint.EUWest3);

var cart = new ShoppingCart()
{
    Id = Guid.NewGuid(),
    ProductName = "Product 1"
};
var order = new Order()
{
    Id = Guid.NewGuid(),
    ProductName = "Product 1"
};

var cartJson = JsonSerializer.Serialize(cart);
var orderJson = JsonSerializer.Serialize(order);

var attributeMapCart = Document.FromJson(cartJson).ToAttributeMap();
var attributeMapOrder = Document.FromJson(orderJson).ToAttributeMap();

var transactionRequest = new TransactWriteItemsRequest
{
    TransactItems = new List<TransactWriteItem>
    {
        new TransactWriteItem
        {
            Put = new Put
            {
                TableName = "shopping-carts",
                Item = attributeMapCart
            }
        },
        new TransactWriteItem
        {
            Put = new Put
            {
                TableName = "orders",
                Item = attributeMapOrder
            }
        }
    }
};

var response = await client.TransactWriteItemsAsync(transactionRequest);