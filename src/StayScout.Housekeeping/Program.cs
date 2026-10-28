using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client;
using StayScout.SearchService.Infrastructure.Persistence;

var builder = Host.CreateApplicationBuilder(args);

var rabbitMqUri = builder.Configuration["RabbitMq:Uri"]
    ?? throw new InvalidOperationException(
        "RabbitMq:Uri is not configured.");

var connectionString = builder.Configuration
    .GetConnectionString("SearchDatabase")
    ?? throw new InvalidOperationException(
        "Connection string 'SearchDatabase' is not configured.");

builder.Services.AddDbContext<SearchDbContext>(options =>
    options.UseNpgsql(connectionString));

using var host = builder.Build();

await using (var scope = host.Services.CreateAsyncScope())
{
    var dbContext = scope.ServiceProvider
        .GetRequiredService<SearchDbContext>();

    await dbContext.Database.MigrateAsync();
}

Console.WriteLine("Database migrated successfully.");

var factory = new ConnectionFactory
{
    Uri = new Uri(rabbitMqUri)
};

await using var connection =
    await factory.CreateConnectionAsync();

await using var channel =
    await connection.CreateChannelAsync();

async Task ProvisionAsync(string exchange, string queue)
{
    await channel.ExchangeDeclareAsync(
        exchange: exchange,
        type: ExchangeType.Fanout,
        durable: true,
        autoDelete: false);

    await channel.QueueDeclareAsync(
        queue: queue,
        durable: true,
        exclusive: false,
        autoDelete: false);

    await channel.QueueBindAsync(
        queue: queue,
        exchange: exchange,
        routingKey: string.Empty);
}

await ProvisionAsync("search-requested", "search-requests");
await ProvisionAsync("search-completed", "search-completed-notifications");

Console.WriteLine("RabbitMQ topology provisioned successfully.");

return 0;
