using MassTransit;
using StayScout.Contracts;
using StayScout.SearchService.Abstractions;
using StayScout.SearchService.Infrastructure.Messaging;

namespace StayScout.SearchService;

public static class DependencyInjection
{
    public static IServiceCollection AddSearchMessaging(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var rabbitMqUri = configuration["RabbitMq:Uri"]
            ?? throw new InvalidOperationException(
                "RabbitMq:Uri is not configured.");

        services.AddMassTransit(registration =>
        {
            registration.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host(new Uri(rabbitMqUri));

                cfg.Message<SearchRequested>(message =>
                {
                    message.SetEntityName("search-requested");
                });

                cfg.Publish<SearchRequested>(publish =>
                {
                    publish.ExchangeType = RabbitMQ.Client.ExchangeType.Fanout;
                });
            });
        });

        services.AddScoped<
            ISearchRequestPublisher,
            MassTransitSearchRequestPublisher>();

        return services;
    }
}
