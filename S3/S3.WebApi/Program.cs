using Amazon.Runtime;
using Amazon.S3;
using Microsoft.AspNetCore.Mvc;
using S3.WebApi.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddSingleton<IAmazonS3>(sp =>
{
    var config = sp.GetRequiredService<IConfiguration>();
    var accessKey = config["AwsOptions:AccessKey"];
    var secretKey = config["AwsOptions:SecretKey"];

    var credentials = new BasicAWSCredentials(accessKey, secretKey);
    return new AmazonS3Client(credentials, Amazon.RegionEndpoint.EUWest3);
});
builder.Services.AddScoped<CustomerService>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}


app.MapPost("/api/image", async ([FromForm] IFormFile file, CustomerService customerService, CancellationToken cancellationToken) =>
{
    var id = await customerService.UploadImageAsync(file, cancellationToken);
    return Results.Created($"/api/image/{id}", id);
}).DisableAntiforgery();
app.MapDelete("/api/image/{id}", async (Guid id, CustomerService customerService, CancellationToken cancellationToken) =>
{
    var deleted = await customerService.DeleteImageAsync(id, cancellationToken);
    return deleted ? Results.Ok() : Results.NotFound();
});
app.MapGet("/api/image/{id}", async (Guid id, CustomerService customerService, CancellationToken cancellationToken) =>
{
    var response = await customerService.GetImageAsync(id, cancellationToken);
    return response is not null ? Results.File(response.ResponseStream, response.Headers.ContentType ?? "application/octet-stream") : Results.NotFound();
});

app.Run();

