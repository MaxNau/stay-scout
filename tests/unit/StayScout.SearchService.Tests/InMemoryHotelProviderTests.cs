using StayScout.SearchService.Providers;

namespace StayScout.SearchService.Tests;

public sealed class InMemoryHotelProviderTests
{
    [Test]
    public async Task GetHotels_ReturnsAtLeastSixHotels()
    {
        // Arrange
        var sut = new InMemoryHotelProvider();

        // Act
        var hotels = sut.GetHotels(string.Empty);

        // Assert
        await Assert.That(hotels.Count).IsGreaterThanOrEqualTo(6);
    }

    [Test]
    public async Task GetHotels_ReturnsUniqueHotelIds()
    {
        // Arrange
        var sut = new InMemoryHotelProvider();

        // Act
        var hotels = sut.GetHotels(string.Empty);
        var distinctIdCount = hotels.Select(hotel => hotel.Id).Distinct().Count();

        // Assert
        await Assert.That(distinctIdCount).IsEqualTo(hotels.Count);
    }

    [Test]
    public async Task GetHotels_RepeatedCalls_ReturnSameResultsInSameOrder()
    {
        // Arrange
        var sut = new InMemoryHotelProvider();
        const string query = "Kyiv";

        // Act
        var first = sut.GetHotels(query);
        var second = sut.GetHotels(query);

        var firstResults = first.Select(hotel =>
            (hotel.Id, hotel.Name, hotel.City, hotel.Address, hotel.PricePerNight));

        var secondResults = second.Select(hotel =>
            (hotel.Id, hotel.Name, hotel.City, hotel.Address, hotel.PricePerNight));

        // Assert
        await Assert.That(firstResults.SequenceEqual(secondResults)).IsTrue();
    }

    [Test]
    public async Task GetHotels_QueryMatchesCity_CaseInsensitively()
    {
        // Arrange
        var sut = new InMemoryHotelProvider();
        const string query = "kYiV";

        // Act
        var hotels = sut.GetHotels(query);
        var matchingCount = hotels.TakeWhile(hotel =>
            hotel.City.Contains(query, StringComparison.OrdinalIgnoreCase) ||
            hotel.Name.Contains(query, StringComparison.OrdinalIgnoreCase))
            .Count();

        // Assert
        await Assert.That(matchingCount).IsGreaterThan(0);
        await Assert.That(hotels.Take(matchingCount).All(hotel =>
            hotel.City.Contains(query, StringComparison.OrdinalIgnoreCase) ||
            hotel.Name.Contains(query, StringComparison.OrdinalIgnoreCase)))
            .IsTrue();
    }
}
