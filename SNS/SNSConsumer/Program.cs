using Amazon.SimpleNotificationService;
using Microsoft.Extensions.Hosting;
using SNS.Shared;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddSharedServices(builder.Configuration);

using var host = builder.Build();
var snsClient = host.Services.GetRequiredService<IAmazonSimpleNotificationService>();

var topicArnResponse = await snsClient.FindTopicAsync("customers");
var recieveMessageRequest = new ReceiveMessageRequest
{
    TopicArn = topicArnResponse.TopicArn,
    MessageAttributeNames = new List<string>() { "All" },
};

var cts = new CancellationTokenSource();
while (!cts.Token.IsCancellationRequested)
{
    var response = await snsClient.ReceiveMessageAsync(recieveMessageRequest, cts.Token);
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
