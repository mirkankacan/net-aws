using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.Extensions.Hosting;

var builder = Host.CreateApplicationBuilder(args);
using var host = builder.Build();

var client = new AmazonS3Client(builder.Configuration["AwsOptions:AccessKey"], builder.Configuration["AwsOptions:SecretKey"], Amazon.RegionEndpoint.EUWest3);
var localFilePath = Path.Combine(AppContext.BaseDirectory, "rembr.jpg");
using var stream = new FileStream(localFilePath, FileMode.Open, FileAccess.Read);
var putObjectRequest = new PutObjectRequest()
{
    BucketName = builder.Configuration["AwsOptions:BucketName"],
    Key = $"images/{Guid.NewGuid()}.jpg",
    ContentType = "image/jpeg",
    InputStream = stream
};
await client.PutObjectAsync(putObjectRequest);