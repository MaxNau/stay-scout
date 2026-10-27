namespace StayScout.SearchService.Providers;

public sealed class InMemoryHotelProvider : IHotelProvider
{
    private static readonly Hotel[] Hotels =
    [
        new("1", "Hotel Central", "Kyiv", "Khreshchatyk 1", 120),
        new("2", "Grand Hotel", "Lviv", "Svobody Avenue 10", 150),
        new("3", "City Hotel", "Odesa", "Deribasivska 5", 95),
        new("4", "Park Hotel", "Kyiv", "Shevchenka 20", 110),
        new("5", "Riverside Hotel", "Dnipro", "Naberezhna 8", 85),
        new("6", "Old Town Hotel", "Lviv", "Rynok Square 3", 135),
        new("7", "Sunrise Hotel", "Kharkiv", "Sumska 12", 80),
        new("8", "Opera Hotel", "Odesa", "Rishelievska 7", 140),
        new("9", "Dnipro View Hotel", "Kyiv", "Naberezhno-Khreshchatytska 15", 125),
        new("10", "Castle Hotel", "Lviv", "Lystopadovoho Chynu 4", 160),
        new("11", "Maidan Hotel", "Kyiv", "Instytutska 9", 175),
        new("12", "Seaside Hotel", "Odesa", "Lanzheronivska 22", 130),
        new("13", "Garden Hotel", "Kharkiv", "Pushkinska 30", 70),
        new("14", "Boulevard Hotel", "Dnipro", "Yavornytskoho Avenue 40", 90),
        new("15", "Tram Hotel", "Lviv", "Virmenska 11", 105),
        new("16", "Podil Hotel", "Kyiv", "Sahaidachnoho 25", 100),
        new("17", "Harbor Hotel", "Odesa", "Prymorskyi Boulevard 6", 155),
        new("18", "Freedom Hotel", "Kharkiv", "Freedom Square 2", 95),
        new("19", "Steel City Hotel", "Dnipro", "Hlinky 14", 75),
        new("20", "Coffee House Hotel", "Lviv", "Kopernyka 18", 115),
        new("21", "Golden Gate Hotel", "Kyiv", "Volodymyrska 33", 180),
        new("22", "Black Sea Hotel", "Odesa", "Havanna 1", 110),
        new("23", "Station Hotel", "Kharkiv", "Konstytutsii Square 5", 65),
        new("24", "Island Hotel", "Dnipro", "Monastyrskyi Island 1", 120),
        new("25", "Carpathian Hotel", "Lviv", "Shevchenka 40", 125),
        new("26", "Arsenal Hotel", "Kyiv", "Hrushevskoho 28", 145),
        new("27", "Catacombs Hotel", "Odesa", "Uspenska 15", 90),
        new("28", "University Hotel", "Kharkiv", "Nauky Avenue 8", 85),
        new("29", "Cathedral Hotel", "Dnipro", "Soborna Square 3", 95),
        new("30", "Market Hotel", "Lviv", "Krakivska 20", 100)
    ];

    public IReadOnlyList<Hotel> GetHotels(string query) =>
        [.. Hotels.OrderByDescending(hotel => Matches(hotel, query))];

    private static bool Matches(Hotel hotel, string query) =>
        hotel.City.Contains(query, StringComparison.OrdinalIgnoreCase) ||
        hotel.Name.Contains(query, StringComparison.OrdinalIgnoreCase);
}
