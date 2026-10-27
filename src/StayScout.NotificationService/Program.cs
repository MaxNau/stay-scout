using MassTransit;
using StayScout.Contracts.Events;
using StayScout.SearchService.Infrastructure.Messaging;

var builder = Host.CreateApplicationBuilder(args);

var rabbitMqUri = builder.Configuration["RabbitMq:Uri"]
    ?? throw new InvalidOperationException(
        "RabbitMq:Uri is not configured.");

builder.Services.AddMassTransit(registration =>
{
    registration.AddConsumer<SearchCompletedConsumer>();

    registration.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(new Uri(rabbitMqUri));

        cfg.Message<SearchCompleted>(message =>
        {
            message.SetEntityName("search-completed");
        });

        cfg.ReceiveEndpoint("search-completed", endpoint =>
        {
            endpoint.ConfigureConsumer<SearchCompletedConsumer>(context);
        });
    });
});

var host = builder.Build();
await host.RunAsync();
