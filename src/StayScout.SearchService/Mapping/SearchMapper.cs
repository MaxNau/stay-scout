using Riok.Mapperly.Abstractions;
using StayScout.SearchService.Abstractions.Models;
using StayScout.SearchService.Domain;

namespace StayScout.SearchService.Mapping;

[Mapper]
public static partial class SearchMapper
{
    public static SearchDetails ToDetails(
        Search search,
        IReadOnlyList<SearchResult> results) =>
        new(
            search.Id,
            search.Status == SearchStatus.Completed,
            [.. results.Select(ToDetails)]);

    [MapProperty(
        nameof(SearchResult.ProviderResultId),
        nameof(SearchResultDetails.Id))]
    [MapperIgnoreSource(nameof(SearchResult.Id))]
    [MapperIgnoreSource(nameof(SearchResult.SearchId))]
    public static partial SearchResultDetails ToDetails(
        SearchResult source);
}
