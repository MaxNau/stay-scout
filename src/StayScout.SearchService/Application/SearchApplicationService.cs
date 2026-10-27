using StayScout.SearchService.Abstractions;
using StayScout.SearchService.Abstractions.Models;
using StayScout.SearchService.Domain;
using StayScout.SearchService.Infrastructure.Persistence;
using StayScout.SearchService.Mapping;
using StayScout.SearchService.Providers;

namespace StayScout.SearchService.Application;

public sealed class SearchApplicationService(
    ISearchDbAccess dbAccess,
    ISearchRequestPublisher searchRequestPublisher,
    ISearchCompletedPublisher searchCompletedPublisher,
    IHotelProvider hotelProvider)
    : ISearchApplicationService
{
    private const int TotalBatches = 6;

    private static readonly TimeSpan BatchInterval =
        TimeSpan.FromSeconds(5);

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

    public async Task ProcessSearchAsync(
        Guid searchId,
        CancellationToken cancellationToken)
    {
        var search = await dbAccess.GetSearchAsync(
            searchId,
            cancellationToken) ?? throw new InvalidOperationException(
                $"Search '{searchId}' does not exist.");

        if (search.Status is SearchStatus.Completed or SearchStatus.Failed)
        {
            return;
        }

        var hotels = hotelProvider.GetHotels(search.Query).ToArray();

        if (hotels.Length < TotalBatches)
        {
            throw new InvalidOperationException(
                $"Provider returned {hotels.Length} hotels, at least {TotalBatches} are required.");
        }

        for (var batchNumber = search.CompletedBatches + 1;
             batchNumber <= TotalBatches;
             batchNumber++)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (batchNumber > 1)
            {
                await Task.Delay(BatchInterval, cancellationToken);
            }

            var batchResults = GetBatch(hotels, batchNumber)
                .Select(hotel => new SearchResult
                {
                    Id = Guid.NewGuid(),
                    SearchId = searchId,
                    ProviderResultId = hotel.Id,
                    Name = hotel.Name,
                    City = hotel.City,
                    Address = hotel.Address,
                    PricePerNight = (decimal)hotel.PricePerNight
                })
                .ToArray();

            await dbAccess.SaveBatchAsync(
                searchId,
                batchNumber,
                batchNumber == TotalBatches,
                batchResults,
                cancellationToken);
        }

        var completedSearch = await dbAccess.GetSearchAsync(
            searchId,
            cancellationToken);

        if (completedSearch?.Status == SearchStatus.Completed)
        {
            await searchCompletedPublisher.PublishAsync(
                searchId,
                cancellationToken);
        }
    }

    private static T[] GetBatch<T>(T[] items, int batchNumber) =>
        items[((batchNumber - 1) * items.Length / TotalBatches)..(batchNumber * items.Length / TotalBatches)];
}
