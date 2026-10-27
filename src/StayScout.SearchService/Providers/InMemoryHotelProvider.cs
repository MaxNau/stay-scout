namespace StayScout.SearchService.Providers;

public sealed class InMemoryHotelProvider : IHotelProvider
{
    public IReadOnlyList<Hotel> GetHotels() =>
    [
        new("1", "Hotel Central", "Kyiv", "Khreshchatyk 1", 120),
        new("2", "Grand Hotel", "Lviv", "Svobody Avenue 10", 150),
        new("3", "City Hotel", "Odesa", "Deribasivska 5", 95),
        new("4", "Park Hotel", "Kyiv", "Shevchenka 20", 110),
        new("5", "Riverside Hotel", "Dnipro", "Naberezhna 8", 85),
        new("6", "Old Town Hotel", "Lviv", "Rynok Square 3", 135)
    ];
}
