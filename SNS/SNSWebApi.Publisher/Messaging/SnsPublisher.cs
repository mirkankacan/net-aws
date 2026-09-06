using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;
using System.Text.Json;

namespace SNSWebApi.Publisher.Messaging
{
    public sealed class SnsPublisher(IAmazonSimpleNotificationService snsClient)
    {
        public async Task<PublishResponse> PublishAsync<T>(T message, string topicName, string messageType, bool isSingle, CancellationToken cancellationToken = default)
        {
            var topicArnResponse = await snsClient.FindTopicAsync(topicName);
            var publishRequest = new PublishRequest
            {
                TopicArn = topicArnResponse.TopicArn,
                Message = JsonSerializer.Serialize(message),
                MessageAttributes = new Dictionary<string, MessageAttributeValue>
                {
                    { "MessageType", new MessageAttributeValue { DataType = "String", StringValue = messageType } },
                    { "IsSingle", new MessageAttributeValue { DataType = "String", StringValue = isSingle.ToString().ToLower() } }
                }
            };

            var response = await snsClient.PublishAsync(publishRequest, cancellationToken);
            return response;
        }
    }
}
