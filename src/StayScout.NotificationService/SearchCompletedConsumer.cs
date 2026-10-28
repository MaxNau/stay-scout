using MassTransit;
using StayScout.Contracts.Events;

namespace StayScout.SearchService.Infrastructure.Messaging;

public sealed partial class SearchCompletedConsumer(
    ILogger<SearchCompletedConsumer> logger)
    : IConsumer<SearchCompleted>
{
    public Task Consume(ConsumeContext<SearchCompleted> context)
    {
        LogSearchCompleted(logger, context.Message.SearchId);
        return Task.CompletedTask;
    }

    [LoggerMessage(
        EventId = 1,
        Level = LogLevel.Information,
        Message = "Search {SearchId} completed.")]
    private static partial void LogSearchCompleted(
        ILogger logger,
        string searchId);
}
