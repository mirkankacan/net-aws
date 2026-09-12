using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.Extensions.Hosting;
using System.Text;

var builder = Host.CreateApplicationBuilder(args);
using var host = builder.Build();

var client = new AmazonS3Client(builder.Configuration["AwsOptions:AccessKey"], builder.Configuration["AwsOptions:SecretKey"], Amazon.RegionEndpoint.EUWest3);

var getObjectRequest = new GetObjectRequest()
{
    BucketName = builder.Configuration["AwsOptions:BucketName"],
    Key = "images/9aa7db04-6c57-4e29-bcbd-29ccb792ef69.jpg"
};

var response = await client.GetObjectAsync(getObjectRequest);
using var memoryStream = new MemoryStream();

await response.ResponseStream.CopyToAsync(memoryStream);

var text = Encoding.Default.GetString(memoryStream.ToArray());

Console.WriteLine(text);