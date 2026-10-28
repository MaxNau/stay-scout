using Riok.Mapperly.Abstractions;
using StayScout.Contracts.Grpc;
using StayScout.Gateway.Dtos;

namespace StayScout.Gateway.Mappers;

[Mapper]
public static partial class SearchMapper
{
    public static partial StartSearchRequest ToGrpc(
        StartSearchRequestDto source);

    [MapProperty(
        nameof(StartSearchResponse.SearchId),
        nameof(StartSearchResponseDto.SearchId),
        SuppressNullMismatchDiagnostic = true)]
    public static partial StartSearchResponseDto ToDto(
        StartSearchResponse source);

    [MapProperty(
        nameof(SearchResult.Id),
        nameof(SearchResultResponseDto.Id),
        SuppressNullMismatchDiagnostic = true)]
    [MapProperty(
        nameof(SearchResult.Name),
        nameof(SearchResultResponseDto.Name),
        SuppressNullMismatchDiagnostic = true)]
    [MapProperty(
        nameof(SearchResult.City),
        nameof(SearchResultResponseDto.City),
        SuppressNullMismatchDiagnostic = true)]
    [MapProperty(
        nameof(SearchResult.Address),
        nameof(SearchResultResponseDto.Address),
        SuppressNullMismatchDiagnostic = true)]
    public static partial SearchResultResponseDto ToDto(
        SearchResult source);

    public static SearchDetailsResponseDto ToDto(
        GetSearchResponse source) =>
        new(
            source.SearchId,
            source.IsCompleted,
            [.. source.Results.Select(ToDto)]);
}
