using StayScout.SearchService.Abstractions;
using StayScout.SearchService.Application;
using StayScout.SearchService.Domain;
using StayScout.SearchService.Infrastructure.Persistence;
using StayScout.SearchService.Providers;

namespace StayScout.SearchService.Tests;

public sealed class SearchApplicationServiceTests
{
    [Test]
    public async Task StartSearchAsync_CreatesSearchAndPublishesItsId()
    {
        // Arrange
        var searchId = Guid.NewGuid();
        const string query = "Kyiv";

        var dbAccess = ISearchDbAccess.Mock(MockBehavior.Strict);
        var requestPublisher = ISearchRequestPublisher.Mock(MockBehavior.Strict);
        var completedPublisher = ISearchCompletedPublisher.Mock(MockBehavior.Strict);
        var hotelProvider = IHotelProvider.Mock(MockBehavior.Strict);

        dbAccess.CreateSearchAsync(query, CancellationToken.None)
            .Returns(new Search { Id = searchId, Query = query });

        requestPublisher.PublishAsync(searchId, CancellationToken.None)
            .Returns(() => Task.CompletedTask);

        var sut = new SearchApplicationService(
            dbAccess,
            requestPublisher,
            completedPublisher,
            hotelProvider);

        // Act
        var result = await sut.StartSearchAsync(
            "  Kyiv  ",
            CancellationToken.None);

        // Assert
        await Assert.That(result).IsEqualTo(searchId);

        dbAccess.CreateSearchAsync(query, CancellationToken.None)
            .WasCalled(Times.Once);

        requestPublisher.PublishAsync(searchId, CancellationToken.None)
            .WasCalled(Times.Once);
    }

    [Test]
    public async Task GetSearchAsync_UnknownSearch_ReturnsNull()
    {
        // Arrange
        var searchId = Guid.NewGuid();

        var dbAccess = ISearchDbAccess.Mock(MockBehavior.Strict);
        var requestPublisher = ISearchRequestPublisher.Mock(MockBehavior.Strict);
        var completedPublisher = ISearchCompletedPublisher.Mock(MockBehavior.Strict);
        var hotelProvider = IHotelProvider.Mock(MockBehavior.Strict);

        dbAccess.GetSearchAsync(searchId, CancellationToken.None)
            .Returns((Search?)null);

        var sut = new SearchApplicationService(
            dbAccess,
            requestPublisher,
            completedPublisher,
            hotelProvider);

        // Act
        var result = await sut.GetSearchAsync(
            searchId,
            CancellationToken.None);

        // Assert
        await Assert.That(result).IsNull();

        dbAccess.GetSearchAsync(searchId, CancellationToken.None)
            .WasCalled(Times.Once);
    }
}
