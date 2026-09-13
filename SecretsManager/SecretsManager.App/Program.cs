using Amazon.SecretsManager;
using Amazon.SecretsManager.Model;
using Microsoft.Extensions.Hosting;
using System.Text.Json;

var builder = Host.CreateApplicationBuilder(args);
using var host = builder.Build();

var client = new AmazonSecretsManagerClient(builder.Configuration["AwsOptions:AccessKey"], builder.Configuration["AwsOptions:SecretKey"], Amazon.RegionEndpoint.EUWest3);

var request = new GetSecretValueRequest()
{
    SecretId = "dev/netaws/key"
};

var response = await client.GetSecretValueAsync(request);
Console.WriteLine(response.SecretString);
Console.WriteLine("            ");
var describeSecretRequest = new DescribeSecretRequest()
{
    SecretId = "dev/netaws/key"
};
var describeSecretResponse = await client.DescribeSecretAsync(describeSecretRequest);
Console.WriteLine($"Secret Details: {JsonSerializer.Serialize(describeSecretResponse)}");
Console.WriteLine("            ");


var requestPrev = new GetSecretValueRequest()
{
    SecretId = "dev/netaws/key",
    VersionStage = "AWSPREVIOUS"
};

var responsePrev = await client.GetSecretValueAsync(requestPrev);
Console.WriteLine(responsePrev.SecretString);
Console.WriteLine("            ");


var listSecretVersionRequest = new ListSecretVersionIdsRequest()
{
    SecretId = "dev/netaws/key",
    IncludeDeprecated = true
};
var versionResponse = await client.ListSecretVersionIdsAsync(listSecretVersionRequest);
Console.WriteLine(JsonSerializer.Serialize(versionResponse));
Console.WriteLine("            ");



var requestVer = new GetSecretValueRequest()
{
    SecretId = "dev/netaws/key",
    VersionId = "58fab67a-8560-447b-904c-9dd5ea7a5ae8"
};

var responseVer = await client.GetSecretValueAsync(requestVer);
Console.WriteLine(responseVer.SecretString);
