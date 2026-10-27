namespace StayScout.SearchService.Domain;

public sealed class Search
{
    public Guid Id { get; set; }

    public required string Query { get; set; }

    public SearchStatus Status { get; set; }

    public int CompletedBatches { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    public DateTimeOffset UpdatedAt { get; set; }
}
