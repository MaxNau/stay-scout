using Microsoft.EntityFrameworkCore;
using StayScout.SearchService.Domain;

namespace StayScout.SearchService.Infrastructure.Persistence;

public sealed class SearchDbAccess(SearchDbContext dbContext) : ISearchDbAccess
{
    public async Task<Search> CreateSearchAsync(
        string query,
        CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(query);

        var now = DateTimeOffset.UtcNow;

        var search = new Search
        {
            Id = Guid.NewGuid(),
            Query = query.Trim(),
            Status = SearchStatus.Pending,
            CompletedBatches = 0,
            CreatedAt = now,
            UpdatedAt = now
        };

        dbContext.Searches.Add(search);

        await dbContext.SaveChangesAsync(cancellationToken);

        return search;
    }

    public Task<Search?> GetSearchAsync(
        Guid searchId,
        CancellationToken cancellationToken) =>
        dbContext.Searches
            .AsNoTracking()
            .SingleOrDefaultAsync(
                search => search.Id == searchId,
                cancellationToken);

    public async Task<IReadOnlyList<SearchResult>> GetResultsAsync(
        Guid searchId,
        CancellationToken cancellationToken) =>
        await dbContext.SearchResults
            .AsNoTracking()
            .Where(result => result.SearchId == searchId)
            .ToListAsync(cancellationToken);
}
