using Amazon.SQS;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace SQS.Shared
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddSharedServices(this IServiceCollection services, IConfiguration configuration)
        {
            AddSqs(services);
            return services;
        }

        private static void AddSqs(IServiceCollection services)
        {
            services.AddSingleton<IAmazonSQS>(sp =>
            {
                var configuration = sp.GetRequiredService<IConfiguration>();
                var accessKey = configuration["AwsSqsOptions:AccessKey"];
                var secretKey = configuration["AwsSqsOptions:SecretKey"];
                var credentials = new Amazon.Runtime.BasicAWSCredentials(accessKey, secretKey);
                return new AmazonSQSClient(credentials, Amazon.RegionEndpoint.EUWest3);
            });
        }
    }
}
