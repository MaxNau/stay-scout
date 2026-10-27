namespace StayScout.SearchService.Abstractions;

public interface ISearchCompletedPublisher
{
    Task PublishAsync(Guid searchId, CancellationToken cancellationToken);
}
