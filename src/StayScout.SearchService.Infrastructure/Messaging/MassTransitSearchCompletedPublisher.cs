using MassTransit;
using StayScout.Contracts.Events;
using StayScout.SearchService.Abstractions;

namespace StayScout.SearchService.Infrastructure.Messaging;

public sealed class MassTransitSearchCompletedPublisher(
    IPublishEndpoint publishEndpoint)
    : ISearchCompletedPublisher
{
    public Task PublishAsync(
        Guid searchId,
        CancellationToken cancellationToken) =>
        publishEndpoint.Publish(
            new SearchCompleted(searchId.ToString(), DateTimeOffset.UtcNow),
            cancellationToken);
}
