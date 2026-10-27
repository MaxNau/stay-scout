namespace StayScout.SearchService.Providers;

public interface IHotelProvider
{
    IReadOnlyList<Hotel> GetHotels(string query);
}
