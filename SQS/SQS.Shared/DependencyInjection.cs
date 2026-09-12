using Amazon.SQS;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace SQS.Shared
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddSharedServices(this IServiceCollection services, IConfiguration configuration)
        {
            AddSqs(services, configuration);
            return services;
        }

        private static void AddSqs(IServiceCollection services, IConfiguration configuration)
        {
            var accessKey = configuration["AwsOptions:AccessKey"];
            var secretKey = configuration["AwsOptions:SecretKey"];
            var credentials = new Amazon.Runtime.BasicAWSCredentials(accessKey, secretKey);

            services.AddSingleton<IAmazonSQS>(_ => new AmazonSQSClient(credentials, Amazon.RegionEndpoint.EUWest3));
        }
    }
}
