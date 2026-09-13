using Amazon.Lambda.Core;
using Amazon.Lambda.DynamoDBEvents;
using System.Text.Json;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace LambdaDynamoDb
{
    public class Function
    {
        public void FunctionHandler(DynamoDBEvent dynamoEvent, ILambdaContext context)
        {
            context.Logger.LogInformation($"Beginning to process {dynamoEvent.Records.Count} records...");

            foreach (var record in dynamoEvent.Records)
            {
                context.Logger.LogInformation($"Event ID: {record.EventID}");
                context.Logger.LogInformation($"Event Name: {record.EventName}");
                context.Logger.LogInformation($"New Image: {JsonSerializer.Serialize(record.Dynamodb.NewImage)}");
                // TODO: Add business logic processing the record.Dynamodb object.
            }

            context.Logger.LogInformation("Stream processing complete.");
        }
    }
}