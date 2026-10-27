using StayScout.SearchService.Abstractions;
using StayScout.SearchService.Abstractions.Models;
using StayScout.SearchService.Infrastructure.Persistence;
using StayScout.SearchService.Mapping;

namespace StayScout.SearchService.Application;

public sealed class SearchApplicationService(
    ISearchDbAccess dbAccess,
    ISearchRequestPublisher searchRequestPublisher)
    : ISearchApplicationService
{
    public async Task<Guid> StartSearchAsync(
        string query,
        CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(query);

        var search = await dbAccess.CreateSearchAsync(
            query.Trim(),
            cancellationToken);

        await searchRequestPublisher.PublishAsync(
            search.Id,
            cancellationToken);

        return search.Id;
    }

    public async Task<SearchDetails?> GetSearchAsync(
        Guid searchId,
        CancellationToken cancellationToken)
    {
        var search = await dbAccess.GetSearchAsync(
            searchId,
            cancellationToken);

        if (search is null)
        {
            return null;
        }

        var results = await dbAccess.GetResultsAsync(
            searchId,
            cancellationToken);

        return SearchMapper.ToDetails(search, results);
    }
}
