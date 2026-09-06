using Amazon.SQS;
using Amazon.SQS.Model;
using System.Text.Json;

namespace SQSWebApi.Publisher.Messaging
{
    public sealed class SqsPublisher(IAmazonSQS sqsClient)
    {
        public async Task<SendMessageResponse> SendMessageAsync<T>(T message, string queueName, string messageType, CancellationToken cancellationToken = default)
        {
            var queueUrlResponse = await sqsClient.GetQueueUrlAsync(queueName, cancellationToken);
            var sendMessageRequest = new SendMessageRequest
            {
                QueueUrl = queueUrlResponse.QueueUrl,
                MessageBody = JsonSerializer.Serialize(message),
                MessageAttributes = new Dictionary<string, MessageAttributeValue>
                {
                    { "MessageType", new MessageAttributeValue { DataType = "String", StringValue = messageType } },
                }
            };

            var response = await sqsClient.SendMessageAsync(sendMessageRequest, cancellationToken);
            return response;
        }
    }
}
