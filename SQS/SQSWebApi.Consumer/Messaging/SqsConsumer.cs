using Amazon.SQS;
using Amazon.SQS.Model;
using System.Text.Json;

namespace SQSWebApi.Consumer.Messaging
{
    public sealed class SqsConsumer(IAmazonSQS sqsClient)
    {
        public async Task ReceiveMessageAsync<T>(string queueName, CancellationToken cancellationToken = default)
        {
            var queueUrlResponse = await sqsClient.GetQueueUrlAsync(queueName, cancellationToken);
            var recieveMessageRequest = new ReceiveMessageRequest
            {
                QueueUrl = queueUrlResponse.QueueUrl,
                MessageAttributeNames = new List<string>() { "All" },
                WaitTimeSeconds = 20
            };


            var response = await sqsClient.ReceiveMessageAsync(recieveMessageRequest, cancellationToken);
            foreach (var message in response.Messages ?? Enumerable.Empty<Message>())
            {
                try
                {
                    Console.WriteLine($"Message ID: {message.MessageId}");
                    T? data = JsonSerializer.Deserialize<T>(message.Body);
                    Console.WriteLine($"Received message: {JsonSerializer.Serialize(data)}");
                    Console.WriteLine("Attributes:");
                    foreach (var attr in message.MessageAttributes ?? new Dictionary<string, MessageAttributeValue>())
                    {
                        Console.WriteLine($"  {attr.Key} ({attr.Value.DataType}): {attr.Value.StringValue}");
                    }
                    await sqsClient.DeleteMessageAsync(queueUrlResponse.QueueUrl, message.ReceiptHandle, cancellationToken);
                }
                catch (Exception) { }
            }
        }
    }
}
