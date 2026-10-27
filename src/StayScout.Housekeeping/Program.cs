using RabbitMQ.Client;

var builder = Host.CreateApplicationBuilder(args);

var rabbitMqUri = builder.Configuration["RabbitMq:Uri"]
    ?? throw new InvalidOperationException(
        "RabbitMq:Uri is not configured.");

using var host = builder.Build();

var factory = new ConnectionFactory
{
    Uri = new Uri(rabbitMqUri)
};

await using var connection =
    await factory.CreateConnectionAsync();

await using var channel =
    await connection.CreateChannelAsync();

await channel.ExchangeDeclareAsync(
    exchange: "search-requested",
    type: ExchangeType.Fanout,
    durable: true,
    autoDelete: false);

await channel.QueueDeclareAsync(
    queue: "search-requests",
    durable: true,
    exclusive: false,
    autoDelete: false);

await channel.QueueBindAsync(
    queue: "search-requests",
    exchange: "search-requested",
    routingKey: string.Empty);

Console.WriteLine("RabbitMQ topology provisioned successfully.");

return 0;
