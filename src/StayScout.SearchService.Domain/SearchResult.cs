namespace StayScout.SearchService.Domain;

public sealed class SearchResult
{
    public Guid Id { get; set; }

    public Guid SearchId { get; set; }

    public required string ProviderResultId { get; set; }

    public required string Name { get; set; }

    public required string City { get; set; }

    public required string Address { get; set; }

    public decimal PricePerNight { get; set; }
}
