namespace StayScout.SearchService.Abstractions.Models;

public sealed record SearchResultDetails(
    string Id,
    string Name,
    string City,
    string Address,
    decimal PricePerNight);
