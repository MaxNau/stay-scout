namespace StayScout.SearchService.Abstractions;

public interface ISearchRequestPublisher
{
    Task PublishAsync(
        Guid searchId,
        CancellationToken cancellationToken);
}
