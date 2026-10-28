using StayScout.SearchService.Providers;

namespace StayScout.SearchService.Tests;

public sealed class InMemoryHotelProviderTests
{
    private readonly InMemoryHotelProvider _sut = new();

    [Test]
    public async Task GetHotels_EmptyQuery_ReturnsAtLeastSixHotels()
    {
        var hotels = _sut.GetHotels(string.Empty);

        await Assert.That(hotels.Count).IsGreaterThanOrEqualTo(6);
    }

    [Test]
    public async Task GetHotels_EmptyQuery_ReturnsUniqueIds()
    {
        var hotels = _sut.GetHotels(string.Empty);

        var ids = hotels.Select(hotel => hotel.Id).ToArray();

        await Assert.That(ids.Distinct().Count()).IsEqualTo(ids.Length);
    }

    [Test]
    public async Task GetHotels_RepeatedCalls_ReturnSameResultsInSameOrder()
    {
        var first = _sut.GetHotels("Kyiv");
        var second = _sut.GetHotels("Kyiv");

        await Assert.That(second).IsEquivalentTo(first);
    }

    [Test]
    public async Task GetHotels_QueryMatchesCity_CaseInsensitively()
    {
        var hotels = _sut.GetHotels("kYiV");

        await Assert.That(hotels).IsNotEmpty();

        await Assert.That(
            hotels.All(hotel =>
                hotel.City.Contains("Kyiv", StringComparison.OrdinalIgnoreCase) ||
                hotel.Name.Contains("kYiV", StringComparison.OrdinalIgnoreCase)))
            .IsTrue();
    }

    [Test]
    public async Task GetHotels_MatchingHotels_AreOrderedBeforeNonMatchingHotels()
    {
        var hotels = _sut.GetHotels("Kyiv");

        var matchingCount = hotels.Count(hotel =>
            hotel.City.Contains("Kyiv", StringComparison.OrdinalIgnoreCase) ||
            hotel.Name.Contains("Kyiv", StringComparison.OrdinalIgnoreCase));

        await Assert.That(matchingCount).IsGreaterThan(0);

        await Assert.That(
            hotels.Take(matchingCount).All(hotel =>
                hotel.City.Contains("Kyiv", StringComparison.OrdinalIgnoreCase) ||
                hotel.Name.Contains("Kyiv", StringComparison.OrdinalIgnoreCase)))
            .IsTrue();

        await Assert.That(
            hotels.Skip(matchingCount).All(hotel =>
                !hotel.City.Contains("Kyiv", StringComparison.OrdinalIgnoreCase) &&
                !hotel.Name.Contains("Kyiv", StringComparison.OrdinalIgnoreCase)))
            .IsTrue();
    }
}
