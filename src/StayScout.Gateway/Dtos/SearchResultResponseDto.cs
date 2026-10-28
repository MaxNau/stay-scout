namespace StayScout.Gateway.Dtos;

public sealed record SearchResultResponseDto(
    string Id,
    string Name,
    string City,
    string Address,
    decimal PricePerNight);
