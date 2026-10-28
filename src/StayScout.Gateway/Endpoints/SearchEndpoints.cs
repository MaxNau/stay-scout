using Microsoft.AspNetCore.Mvc;
using StayScout.Contracts.Grpc;
using StayScout.Gateway.Dtos;
using StayScout.Gateway.Mappers;

namespace StayScout.Gateway.Endpoints;

public static class SearchEndpoints
{
    public static IEndpointRouteBuilder MapSearchEndpoints(
        this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/searches");

        group.MapPost("", StartSearchAsync);
        group.MapGet("/{id:guid}", GetSearchAsync);

        return endpoints;
    }

    private static async Task<IResult> StartSearchAsync(
        [FromBody] StartSearchRequestDto request,
        SearchGrpcService.SearchGrpcServiceClient client,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Query))
        {
            return Results.ValidationProblem(
                new Dictionary<string, string[]>
                {
                    ["query"] = ["Query is required."]
                });
        }

        var response = await client.StartSearchAsync(
            SearchMapper.ToGrpc(request),
            cancellationToken: cancellationToken);

        var dto = SearchMapper.ToDto(response);

        return Results.Accepted(
            $"/api/searches/{dto.SearchId}",
            dto);
    }

    private static async Task<IResult> GetSearchAsync(
        Guid id,
        SearchGrpcService.SearchGrpcServiceClient client,
        CancellationToken cancellationToken)
    {
        var response = await client.GetSearchAsync(
            new GetSearchRequest
            {
                SearchId = id.ToString()
            },
            deadline: DateTime.UtcNow.AddSeconds(3),
            cancellationToken: cancellationToken);

        return Results.Ok(SearchMapper.ToDto(response));
    }
}
