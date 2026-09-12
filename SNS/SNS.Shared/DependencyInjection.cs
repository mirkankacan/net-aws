using Amazon.Runtime;
using Amazon.SimpleNotificationService;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace SNS.Shared
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddSharedServices(this IServiceCollection services, IConfiguration configuration)
        {
            AddSns(services, configuration);
            return services;
        }

        private static void AddSns(IServiceCollection services, IConfiguration configuration)
        {
            var accessKey = configuration["AwsOptions:AccessKey"];
            var secretKey = configuration["AwsOptions:SecretKey"];
            var credentials = new BasicAWSCredentials(accessKey, secretKey);

            services.AddSingleton<IAmazonSimpleNotificationService>(_ => new AmazonSimpleNotificationServiceClient(credentials, Amazon.RegionEndpoint.EUWest3));
        }
    }
}
