namespace StayScout.SearchService.Abstractions.Models;

public sealed record SearchDetails(
    Guid Id,
    bool IsCompleted,
    IReadOnlyList<SearchResultDetails> Results);
