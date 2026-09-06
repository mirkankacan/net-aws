using SQS.Shared;
using SQSWebApi.Consumer.BackgroundServices;
using SQSWebApi.Consumer.Messaging;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddSharedServices(builder.Configuration);
builder.Services.AddSingleton<SqsConsumer>();
builder.Services.AddHostedService<OrderConsumerBackgroundService>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}





app.Run();
