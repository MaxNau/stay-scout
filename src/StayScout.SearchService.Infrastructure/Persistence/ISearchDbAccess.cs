using StayScout.SearchService.Domain;

namespace StayScout.SearchService.Infrastructure.Persistence;


public interface ISearchDbAccess
{
    Task<Search> CreateSearchAsync(
        string query,
        CancellationToken cancellationToken);

    Task<Search?> GetSearchAsync(
        Guid searchId,
        CancellationToken cancellationToken);

    Task<IReadOnlyList<SearchResult>> GetResultsAsync(
        Guid searchId,
        CancellationToken cancellationToken);

    Task SaveBatchAsync(
        Guid searchId,
        int batchNumber,
        bool isLastBatch,
        IReadOnlyList<SearchResult> results,
        CancellationToken cancellationToken);
}
