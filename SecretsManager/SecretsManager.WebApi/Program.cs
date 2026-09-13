using Microsoft.Extensions.Options;
using SecretsManager.WebApi.Options;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var env = builder.Environment.EnvironmentName;
var appName = builder.Environment.ApplicationName;
builder.Configuration.AddSecretsManagerDiscovery(configure: options =>
{
    options.SecretFilter = entry => entry.Name.StartsWith($"{env}_{appName}");
    options.KeyGenerator = context => context.SecretName.Replace($"{env}_{appName}_", string.Empty).Replace("__", ":");
    options.ReloadInterval = TimeSpan.FromSeconds(10);
});
AddValidatedOptions<ConnectionStrings>(builder.Services);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

static IServiceCollection AddValidatedOptions<TOptions>(IServiceCollection services) where TOptions : class
{
    services.AddOptions<TOptions>()
        .BindConfiguration(typeof(TOptions).Name)
        .ValidateDataAnnotations()
        .ValidateOnStart();
    // for rotations on runtime
    services.AddScoped(sp => sp.GetRequiredService<IOptionsMonitor<TOptions>>().CurrentValue);
    return services;
}
app.MapGet("/api/options", (ConnectionStrings connectionStrings) =>
{
    return Results.Ok(connectionStrings.InMemory);
});
app.Run();

