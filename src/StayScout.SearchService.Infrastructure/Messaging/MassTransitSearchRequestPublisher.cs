using MassTransit;
using StayScout.Contracts;
using StayScout.SearchService.Abstractions;

namespace StayScout.SearchService.Infrastructure.Messaging;

public sealed class MassTransitSearchRequestPublisher(
    IPublishEndpoint publishEndpoint)
    : ISearchRequestPublisher
{
    public Task PublishAsync(
        Guid searchId,
        CancellationToken cancellationToken) =>
        publishEndpoint.Publish(
            new SearchRequested(searchId),
            cancellationToken);
}
