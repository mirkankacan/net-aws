using SQS.Shared.Messaging;
using SQSWebApi.Consumer.Models;

namespace SQSWebApi.Consumer.BackgroundServices
{
    public class OrderConsumerBackgroundService(Messaging.SqsConsumer consumer) : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await consumer.ReceiveMessageAsync<List<Order>>(QueueNames.Customers, stoppingToken);
            }
        }
    }
}
