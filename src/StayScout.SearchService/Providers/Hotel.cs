namespace StayScout.SearchService.Providers;

public sealed record Hotel(
    string Id,
    string Name,
    string City,
    string Address,
    double PricePerNight);
