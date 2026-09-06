using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SNS.Shared;
using System.Text.Json;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddSharedServices(builder.Configuration);
using var host = builder.Build();

var snsClient = host.Services.GetRequiredService<IAmazonSimpleNotificationService>();
var message = new
{
    FirstName = "Mirkan",
    LastName = "Kaçan",
    Age = 26
};
var topicArnResponse = await snsClient.FindTopicAsync("customers");

var publishRequest = new PublishRequest
{
    TopicArn = topicArnResponse.TopicArn,
    Message = JsonSerializer.Serialize(message),
    MessageAttributes = new Dictionary<string, MessageAttributeValue>
    {
        { "MessageType", new MessageAttributeValue { DataType = "String", StringValue = "Customer" } },
    }
};
var response = await snsClient.PublishAsync(publishRequest);
Console.WriteLine($"Message sent. Id: {response.MessageId}");
Console.ReadLine();