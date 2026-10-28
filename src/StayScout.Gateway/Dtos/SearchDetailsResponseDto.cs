namespace StayScout.Gateway.Dtos;

public sealed record SearchDetailsResponseDto(
    string SearchId,
    bool IsCompleted,
    IReadOnlyList<SearchResultResponseDto> Results);
