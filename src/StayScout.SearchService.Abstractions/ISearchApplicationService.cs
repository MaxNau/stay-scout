using StayScout.SearchService.Abstractions.Models;

namespace StayScout.SearchService.Abstractions;

public interface ISearchApplicationService
{
    Task<Guid> StartSearchAsync(
        string query,
        CancellationToken cancellationToken);

    Task<SearchDetails?> GetSearchAsync(
        Guid searchId,
        CancellationToken cancellationToken);
}
