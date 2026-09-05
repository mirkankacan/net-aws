using Amazon.SQS;
using Amazon.SQS.Model;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SQS.Shared;
using System.Text.Json;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddSharedServices(builder.Configuration);

using var host = builder.Build();
var sqsClient = host.Services.GetRequiredService<IAmazonSQS>();

var customer = new
{
    FirstName = "Mirkan",
    LastName = "Kaçan",
    Age = 26
};

var queueUrlResponse = await sqsClient.GetQueueUrlAsync("customers");
var sendMessageRequest = new SendMessageRequest
{
    QueueUrl = queueUrlResponse.QueueUrl,
    MessageBody = JsonSerializer.Serialize(customer),
    MessageAttributes = new Dictionary<string, MessageAttributeValue>
    {
        { "MessageType", new MessageAttributeValue { DataType = "String", StringValue = "Customer" } },
    }
};

var response = await sqsClient.SendMessageAsync(sendMessageRequest);
Console.WriteLine($"Message sent. Id: {response.MessageId}");
Console.ReadLine();
