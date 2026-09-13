using Amazon.Lambda.Core;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

// Deploy: dotnet lambda deploy-function SimpleLambda
// Invoke: dotnet lambda invoke-function SimpleLambda --payload '{""Request"": "Hello, World!""}' 
namespace SimpleLambda
{
    public class Function
    {
        public string FunctionHandler(HelloRequest request, ILambdaContext context)
        {
            context.Logger.LogInformation($"Received request: {request.Request}");
            return $"Hello, {request.Request}!";
        }
    }
    public record HelloRequest(string Request);

}
