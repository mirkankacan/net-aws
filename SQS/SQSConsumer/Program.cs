using Amazon.SQS;
using Amazon.SQS.Model;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SQS.Shared;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddSharedServices(builder.Configuration);

using var host = builder.Build();
var sqsClient = host.Services.GetRequiredService<IAmazonSQS>();

var queueUrlResponse = await sqsClient.GetQueueUrlAsync("customers");
var recieveMessageRequest = new ReceiveMessageRequest
{
    QueueUrl = queueUrlResponse.QueueUrl,
    MessageAttributeNames = new List<string>() { "All" },
};

var cts = new CancellationTokenSource();
while (!cts.Token.IsCancellationRequested)
{
    var response = await sqsClient.ReceiveMessageAsync(recieveMessageRequest, cts.Token);
    foreach (var message in response.Messages ?? Enumerable.Empty<Message>())
    {
        try
        {
            Console.WriteLine($"Message ID: {message.MessageId}");
            Console.WriteLine($"Received message: {message.Body}");
            Console.WriteLine("Attributes:");
            foreach (var attr in message.MessageAttributes ?? new Dictionary<string, MessageAttributeValue>())
            {
                Console.WriteLine($"  {attr.Key} ({attr.Value.DataType}): {attr.Value.StringValue}");
            }
            await sqsClient.DeleteMessageAsync(queueUrlResponse.QueueUrl, message.ReceiptHandle);
        }
        catch (Exception) { }
    }
    await Task.Delay(100, cts.Token);
}
