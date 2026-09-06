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
            AddSns(services);
            return services;
        }

        private static void AddSns(IServiceCollection services)
        {
            services.AddSingleton<IAmazonSimpleNotificationService>(sp =>
            {
                var configuration = sp.GetRequiredService<IConfiguration>();
                var accessKey = configuration["AwsOptions:AccessKey"];
                var secretKey = configuration["AwsOptions:SecretKey"];
                var credentials = new BasicAWSCredentials(accessKey, secretKey);
                return new AmazonSimpleNotificationServiceClient(credentials, Amazon.RegionEndpoint.EUWest3);
            });
        }
    }
}
