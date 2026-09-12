using Amazon.DynamoDBv2;
using Amazon.Runtime;
using DynamoDb.Api.Dtos;
using DynamoDb.Api.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddSingleton<IAmazonDynamoDB>(sp =>
{
    var config = sp.GetRequiredService<IConfiguration>();
    var accessKey = config["AwsOptions:AccessKey"];
    var secretKey = config["AwsOptions:SecretKey"];

    var credentials = new BasicAWSCredentials(accessKey, secretKey);
    return new AmazonDynamoDBClient(credentials, Amazon.RegionEndpoint.EUWest3);
});
builder.Services.AddScoped<CustomerRepository>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapGet("/api/customers", async (CustomerRepository customerRepository) =>
{
    var customers = await customerRepository.GetAsync();
    return Results.Ok(customers);
});
app.MapGet("/api/customers/email/{email}", async (string email, CustomerRepository customerRepository) =>
{
    var customer = await customerRepository.GetByEmailAsync(email);
    return customer is null ? Results.NotFound() : Results.Ok(customer);
});
app.MapPost("/api/customers", async (CreateCustomerDto createCustomerDto, CustomerRepository customerRepository) =>
{
    var id = await customerRepository.CreateAsync(createCustomerDto);
    return Results.Created($"/api/customers/{id}", id);
});
app.MapPut("/api/customers/{id}", async (Guid id, UpdateCustomerDto updateCustomerDto, CustomerRepository customerRepository) =>
{
    var outcome = await customerRepository.UpdateAsync(updateCustomerDto with { Id = id });
    return outcome.Result switch
    {
        UpdateCustomerResult.Success => Results.Ok(outcome.Id),
        UpdateCustomerResult.NotFound => Results.NotFound(),
        UpdateCustomerResult.Conflict => Results.Problem(
            statusCode: StatusCodes.Status409Conflict,
            title: "Customer was modified concurrently",
            detail: "The customer has changed since it was last read. Re-fetch and retry."),
        _ => Results.Problem(statusCode: StatusCodes.Status500InternalServerError)
    };
});
app.MapDelete("/api/customers/{id}", async (Guid id, CustomerRepository customerRepository) =>
{
    var deleted = await customerRepository.DeleteAsync(id);
    return deleted ? Results.NoContent() : Results.NotFound();
});

app.Run();

