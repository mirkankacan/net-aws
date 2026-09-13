using Amazon.Lambda.Annotations;
using Amazon.Lambda.Annotations.APIGateway;
using Amazon.Lambda.Core;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace SimpleLambdaHttp
{
    public class Functions
    {

        public Functions()
        {
        }


        [LambdaFunction]
        [RestApi(LambdaHttpMethod.Get, "/")]
        public IHttpResult Get(ILambdaContext context)
        {
            context.Logger.LogInformation("Handling the 'Get' Request");

            return HttpResults.Ok("Hello AWS Serverless");
        }
    }
}
