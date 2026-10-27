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

    public async Task SaveBatchAsync(
        Guid searchId,
        int batchNumber,
        bool isLastBatch,
        IReadOnlyList<SearchResult> results,
        CancellationToken cancellationToken)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(batchNumber, 1);

        var status = isLastBatch
            ? SearchStatus.Completed
            : SearchStatus.Processing;

        await using var transaction =
            await dbContext.Database.BeginTransactionAsync(cancellationToken);

        var updated = await dbContext.Searches
            .Where(search =>
                search.Id == searchId &&
                search.CompletedBatches == batchNumber - 1 &&
                search.Status != SearchStatus.Completed &&
                search.Status != SearchStatus.Failed)
            .ExecuteUpdateAsync(
                setters => setters
                    .SetProperty(search => search.CompletedBatches, batchNumber)
                    .SetProperty(search => search.Status, status)
                    .SetProperty(search => search.UpdatedAt, DateTimeOffset.UtcNow),
                cancellationToken);

        if (updated == 0)
        {
            var existing = await dbContext.Searches
                .AsNoTracking()
                .SingleOrDefaultAsync(
                    search => search.Id == searchId,
                    cancellationToken)
                ?? throw new InvalidOperationException(
                    $"Search '{searchId}' does not exist.");

            if (existing.CompletedBatches >= batchNumber)
            {
                return;
            }

            throw new InvalidOperationException(
                $"Cannot save batch {batchNumber}: search is {existing.Status} with {existing.CompletedBatches} batches completed.");
        }

        dbContext.SearchResults.AddRange(results);

        await dbContext.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
    }
}
