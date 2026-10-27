using MassTransit;
using StayScout.Contracts;
using StayScout.SearchService.Abstractions;

namespace StayScout.SearchService.Infrastructure.Messaging;

public sealed class SearchRequestedConsumer(
    ISearchApplicationService searchApplicationService)
    : IConsumer<SearchRequested>
{
    public async Task Consume(
        ConsumeContext<SearchRequested> context)
    {
        var searchId = context.Message.SearchId;

        await searchApplicationService.ProcessSearchAsync(
            searchId,
            context.CancellationToken);
    }
}
